using Infrastructure.Microservice.API.Controllers;
using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class GCPInfrastructureControllerTests
{
    private readonly Mock<IGCPInfrastructureServices> _mockService;
    private readonly GCPInfrastructureController _controller;

    public GCPInfrastructureControllerTests()
    {
        _mockService = new Mock<IGCPInfrastructureServices>();
        _controller = new GCPInfrastructureController(_mockService.Object);
    }

    [Fact]
    public void AllCustomerInfrastructureByID_ReturnsOkResult_WhenDataExists()
    {
       
        int customerId = 1;
        
        var infrastructures = new List<GCPInfrastructure>
        {
            new GCPInfrastructure { CUSTOMER_ID = 1, PROJECT_LANGUAJE = "C#", TEMPLATE_USED = "Microservices", DEPLOYMENT_ID = 100 }
        };

        _mockService.Setup(s => s.AllInfrastructureByID(customerId)).Returns(infrastructures);

        
        var result = _controller.AllCustomerInfrastructureByID(customerId);

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedData = Assert.IsType<List<GCPInfrastructure>>(okResult.Value);
        Assert.Single(returnedData);
    }

    [Fact]
    public async Task NewInfrastructure_ReturnsNotFound_WhenTemplateNotFound()
    {
        
        _mockService
            .Setup(s => s.CreateNewInfrastructure(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((FileContentResult)null!); 

       
        var result = await _controller.NewInfrastructure(1, "Python", "UnknownTemplate", 200);

       
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Template not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task NewInfrastructure_ReturnsExpectedResult()
    {
        // Arrange
        var expectedContent = "Infrastructure Created";
        var expectedResult = new FileContentResult(System.Text.Encoding.UTF8.GetBytes(expectedContent), "application/json");

        _mockService
            .Setup(s => s.CreateNewInfrastructure(1, "Java", "SpringBoot", 300))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.NewInfrastructure(1, "Java", "SpringBoot", 300);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        var resultContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
        Assert.Equal("Infrastructure Created", resultContent);
    }



    [Fact]
    public async Task RemoveInfrastructureByID_ReturnsOk_WhenSuccess()
    {
       
        int infrastructureId = 10;
        string expectedResponse = "Infrastructure removed successfully";

        _mockService.Setup(s => s.RemoveNewInfrastructure(infrastructureId)).ReturnsAsync(expectedResponse);

       
        var result = await _controller.RemoveInfrastructureByID(infrastructureId);

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public void AllCustomerInfrastructureByID_ReturnsBadRequest_OnException()
    {
        
        _mockService.Setup(s => s.AllInfrastructureByID(It.IsAny<int>())).Throws(new Exception("Database error"));

        
        var result = _controller.AllCustomerInfrastructureByID(1);

        
        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Database error", badRequest.Value);
    }
}
