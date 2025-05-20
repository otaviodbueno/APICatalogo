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

public class GetProdutoUnitTest : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutoController _controller;

    public GetProdutoUnitTest(ProdutosUnitTestController produtosUnitTestController)
    {
        _controller = new ProdutoController(produtosUnitTestController.repository, produtosUnitTestController.mapper);
    }

    [Fact]
    public async Task GetProdutoById_OkResult()
    {
        //Arrange
        var prodId = 2;

        //Act
        var data = await _controller.Get(prodId);

        //Assert (xunit)
        //var okResult = Assert.IsType<OkObjectResult>(data.Result);
        //Assert.Equal(200, okResult.StatusCode);

        //Assert (fluentAssertions) 
        data.Result.Should().BeOfType<OkObjectResult>() // Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should()
            .Be(200);
    }

    [Fact]
    public async Task GetProdutoById_Return_NotFound()
    {
        //Arrange
        var prodId = 999;

        //Act
        var data = await _controller.Get(prodId);

        //Assert (fluentAssertions) 
        data.Result.Should().BeOfType<NotFoundObjectResult>() // Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should()
            .Be(404);
    }

    [Fact]
    public async Task GetProdutoById_Return_BadRequest()
    {
        //Arrange
        var prodId = -1;

        //Act
        var data = await _controller.Get(prodId);

        //Assert (fluentAssertions) 
        data.Result.Should().BeOfType<BadRequestObjectResult>() // Verifica se o resultado é do tipo OkObjectResult
            .Which.StatusCode.Should()
            .Be(400);
    }

    [Fact]

    public async Task getProdutos_Return_ListOfProdutoDTO()
    {
        // Act
        var data = await _controller.Get();

        // Assert
        data.Result.Should().BeOfType<OkObjectResult>() // Verifica se o resultado é do tipo OkObjectResult
            .Which.Value.Should().BeAssignableTo<IEnumerable<ProdutoDTO>>() // Verifica se o valor é do tipo IEnumerable<ProdutoDTO>
            .And.NotBeNull(); // Verifica se o valor não é nulo

    }
}
