using APICatalogo.Context;
using APICatalogo.DTO.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogoxUnitTest.UnitTest;

public class ProdutosUnitTestController
{
    public IUnitOfWork repository;
    public IMapper mapper;
    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    public static string connectionString = "Server=localhost;DataBase=ApiCatalogoDB;Uid=root;Pwd=2361";

    static ProdutosUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
    }

    public ProdutosUnitTestController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ProdutoDTOMappingProfile());
        });

        mapper = config.CreateMapper();

        var context = new AppDbContext(dbContextOptions);
        repository = new UnitOfWork(context);
    }
}
