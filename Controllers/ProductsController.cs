using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMProductInfoAPI.Data;
using HMProductInfoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;
using HMProductInfoAPI.DTO;
using AutoMapper;
using HMProductInfoAPI.Services;

namespace HMProductInfoAPI.Controllers
{
    //[EnableCors("AllowPolicy")]
    //[Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly HMProductDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly IProductCodeGenerationService _productCodeGenerationService;
        private readonly INotificationService _notificationService;

        

        public ProductsController(HMProductDbContext context, ILogger<ProductsController> logger,
            IHttpClientFactory httpClientFactory, IMapper mapper, IProductCodeGenerationService productCodeGenerationService,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;   
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _productCodeGenerationService = productCodeGenerationService;
            _notificationService = notificationService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct()
        {
            var result =  await _context.Products.ToListAsync();
            return Ok(result.Select(_ => _mapper.Map<ProductDto>(_)));
        }

        // GET: api/Products/5 -- get product by ID 
        [HttpGet]
        [Route("~/api/v1/[controller]/{productId:guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid productId)
        {            
            var product = await _context.Products.FindAsync(productId);
            var result = _mapper.Map<ProductDto>(product);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                //make call to color API 

                #region External API Calls
                if (result.Article.Count > 0)
                {
                    foreach (var article in result.Article)
                    {
                        //validate ColourId before making API call
                        if (ModelState.IsValid && ModelState.ErrorCount > 0 && !ModelState.ContainsKey("ColourId"))
                        {
                            var colourRequest = new HttpRequestMessage(HttpMethod.Get,
                            $"https://dummyuri.hmgroup.com/v1/colours/{article.ColourId}");

                            var colourClient = _httpClientFactory.CreateClient();
                            colourClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            var colourResponse = await colourClient.SendAsync(colourRequest);

                            if (colourResponse.IsSuccessStatusCode)
                            {
                                var responseStream = await colourResponse.Content.ReadAsStreamAsync();

                                var colour = await JsonSerializer.DeserializeAsync<Colour>(responseStream);

                                //create new article 
                                var articleDto = new ArticleDto
                                {
                                    ArticleId = article.ArticleId,
                                    ColourId = article.ColourId,
                                    ArticleName = $"{result.ProductName}-{colour.Name}",
                                    ColourCode = colour.Code,
                                    ColourName = colour.Name
                                };

                                result.Article.Add(articleDto);
                            }

                        }
                    }
                }

                //make call to size API
                if (ModelState.IsValid && ModelState.ErrorCount == 0 && !ModelState.ContainsKey("SizeScaleId"))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://dummyuri.hmgroup.com/v1/sizescale/{product.SizeScaleId}");


                    var client = _httpClientFactory.CreateClient();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                    var response = await client.SendAsync(request);


                    if (response.IsSuccessStatusCode)
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();

                        var size = await JsonSerializer.DeserializeAsync<IEnumerable<Size>>(responseStream);

                        result.Size = size.ToList();
                    }
                }
                #endregion External API Calls

            }
            return Ok(result);
        }


        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody]ProductDto newProduct)
        {
            try
            {
                if (newProduct == null) return BadRequest();

                var product = _mapper.Map<Product>(newProduct);

                /*- ProductName has max 100 character length, if it is longer than 100 , api should send 406 http status 
code with the information of character length problem */
                if (!ModelState.IsValid && ModelState.ErrorCount > 0 && ModelState.ContainsKey("ProductName"))

                {
                    return StatusCode(StatusCodes.Status406NotAcceptable,
                        "Product name length exceeds 100 characters");
                }

                /*- As the owners of sizeScaleId & colourId properties are different teams, we should validate consistency of those dat
a. 
If proper identifiers are not provided, we shouldn't persist and return 400 status code with message. */
                if (!ModelState.IsValid && ModelState.ErrorCount > 0 && ModelState.ContainsKey("SizeScaleId"))

                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        "Invalid sizeScale Identifier");
                }
                if (!ModelState.IsValid && ModelState.ErrorCount > 0 && ModelState.ContainsKey("ColourId"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        "Invalid Colour Identifier");
                }

                /*Each product should have unique productCode. It should be created at backend randomly*/
                string productCode = string.Empty;
                _productCodeGenerationService.GenerateProductCode(product, out productCode);

                product.ProductCode = productCode;

                if(ModelState.IsValid)
                {                   
                    _context.Products.Add(product);

                    await _context.SaveChangesAsync();

                    /*- Send ProductCreatedEvent to notify other teams(Implementation and destionation is up to you) */
                    string productString = JsonSerializer.Serialize(product);

                    await Notify(productString);

                    return CreatedAtAction("PostProduct", new { id = product.ProductId }, newProduct);

                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        "Invalid Product details");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error adding new product to repository");
            }
        }
      
        private async Task<IActionResult> Notify(string productEvent)
        {
            await _notificationService.PushAsync(new(productEvent));
            return Ok();
        }
    }
}
