# HMProductInfoAPI
**************
PRODUCT API 
*************
 
 
The team that is responsible for creating products, needs to create two endpoints. 
Both endpoints will serve for the UI that products are created. They will also be consumed 
by other teams. 
 
First endpoint is for creating product with its articles information. Api 
will accept the payload below: 
 
//For saving product with articles 
{ 
"productId": "guid",//key of the product 
"productName": "string", 
"productYear": "int", 
"channelId": "int", //1:Store 2:Online 3:All Shows where the product will be sold 
"sizeScaleId": "guid",//identifier of sizescale. Data owner is another team 
"articles": [ 
{ 
"articleId": "guid", 
"colourId": "guid"//identifier of colour. Data owner is another team 
} 
] 
} 
 
When we save product information we want below rules to be fulfilled: 
- Each product should have unique productCode. It should be created at backend randomly. Creation of productCode
has 
some rules: 
if channelId = 1 => It will be unique and will be calculated productYear + three digit integer code(2022001,20220
02 etc) 
if channelId = 2 => It will be 6 digit unique alphanumeric code (AAB3A2, 5MG88F, 348XYZ etc) 
if channelId = 3 => It will be integer which increases sequencially and start from 10000000(10000001,10000002,
10000003 etc) 
- We should able to track when a product is created by whom 
- As the owners of sizeScaleId & colourId properties are different teams, we should validate consistency of those dat
a. 
If proper identifiers are not provided, we shouldn't persist and return 400 status code with message. 
- ProductName has max 100 character length, if it is longer than 100 , api should send 406 http status 
code with the information of character length problem 
- Successful persistency should return 201 http status code 
- Send ProductCreatedEvent to notify other teams(Implementation and destionation is up to you) 
 
Second endpoint is for fetching saved product by productId. Api will get productId as input and will return object be
low: 
 
//Response object for getting product by Id 
{ 
"productId": "guid", 
"productName": "string", 
"productCode": "string", 
"sizeScaleId": "guid", 
"createDate": "datetime", 
"createdBy": "string", 
"channelId": "int", 
"articles": [ 
{ 
"articleId": "guid", 
"articleName": "string", 
"colourId": "guid", 
"colourCode": "string", 
"colourName": "string" 
} 
], 
"sizes": [ 
{ 
"sizeId": "guid", 
"sizeName": "string" 
} 
] 
} 
 
When we fetch product, we want below rules to be fulfilled: 
- articleName field is combination of 'productName - colourCode' 
- If productId does not exists, it should return 404 http status code 
- If product is available for given productId, it should return above object with 200 status code 
- Colour properties(colourCode, colourName) in article object should be taken from colour api 
- Size properties(sizeId, sizeName) in sizes collection should be taken from sizescale api 
 
Other specs about application: 
 
- Apis should implement azure ad authentication. Endpoints won't be 
available for everyone(no need to create any azure resource, just implementation) 
- Any unhandled exception should return 500 http status code with exception details. 
- Unhandled exception should be logged centrally(it can be logged to console) 
- All request & response objects should be logged 
- Colour & SizeScale are different team's apis. There should be integration with them.Below you can 
see those api's response objects(Make assumption there are public apis for these endpoints and works without authen
tication) 
- Colour & SizeScale information does not change frequently. Consider performance on integration 
- Project should have clean&layered architecture 
- Project should work on local environment and local sql server. 
- Unit&Integration tests should be written 
- You can consider to use any azure resource if it is necessary. You don't need to create that resource, you can 
make assumption as it is exists. 
- Include yaml build pipeline for azure devops 
- Include ARM template for creating azure resources that you want to use 
 
Sample responses of external apis: 
//https://dummyuri.hmgroup.com/v1/colours/{colourId} 
//Colour api response by colourId 
{ 
"colourId": "guid", 
"colourCode": "string", 
"colourName": "string" 
} 
//https://dummyuri.hmgroup.com/v1/colours 
//Colour api, return all colour codes (8000 item) 
[ 
{ 
"colourId": "guid", 
"colourCode": "string", 
"colourName": "string" 
} 
] 
//https://dummyuri.hmgroup.com/v1/sizescale/{sizeScaleId} 
//SizeScale api return by sizescaleId 
[ 
{ 
"sizeId": "guid", 
"sizeName": "string" 
} 
] 
 
Solution should be created in .NET 6 
