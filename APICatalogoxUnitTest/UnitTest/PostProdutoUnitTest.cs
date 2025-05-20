using APICatalogo.Controllers;
using APICatalogo.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogoxUnitTest.UnitTest;

public class PostProdutoUnitTest : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutoController _controller;
    public PostProdutoUnitTest(ProdutosUnitTestController produtosUnitTestController)
    {
        _controller = new ProdutoController(produtosUnitTestController.repository, produtosUnitTestController.mapper);
    }

    [Fact]
    public async Task PostProduto_Return_CreatedStatusCode()
    {
        //Arrange
        var produto = new ProdutoDTO()
        {
            Nome = "Produto Teste",
            Descricao = "Produto Teste",
            Preco = 10.00M,
            ImageUrl = "http://teste.com/imagem.jpg",
            CategoriaId = 1
        };

        //Act
        var data = await _controller.Post(produto);

        //Assert (fluentAssertions) 
        data.Result.Should().BeOfType<CreatedAtRouteResult>() // Verifica se o resultado é do tipo CreatedAtRouteResult
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task PostProduto_Return_BadRequest()
    {
        //Arrange
        ProdutoDTO prod = null;

        //Act
        var data = await _controller.Post(prod);

        //Assert (fluentAssertions)
        var badRequestResult = data.Result.Should().BeOfType<BadRequestObjectResult>() // Verifica se o resultado é do tipo BadRequestObjectResult
            .Which.StatusCode.Should().Be(400);
    }
}
