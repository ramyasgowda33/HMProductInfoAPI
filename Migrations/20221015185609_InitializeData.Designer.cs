// <auto-generated />
using System;
using HMProductInfoAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HMProductInfoAPI.Migrations
{
    [DbContext(typeof(HMProductDbContext))]
    [Migration("20221015185609_InitializeData")]
    partial class InitializeData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HMProductInfoAPI.Models.Article", b =>
                {
                    b.Property<Guid>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ColourId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ArticleId");

                    b.HasIndex("ProductId");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("HMProductInfoAPI.Models.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProductCode")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("ProductYear")
                        .HasColumnType("int");

                    b.Property<Guid>("SizeScaleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("HMProductInfoAPI.Models.Article", b =>
                {
                    b.HasOne("HMProductInfoAPI.Models.Product", null)
                        .WithMany("Article")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("HMProductInfoAPI.Models.Product", b =>
                {
                    b.Navigation("Article");
                });
#pragma warning restore 612, 618
        }
    }
}
