using Infrastructure.Microservice.API.Controllers;
using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class GCPTemplatesControllerTests
{
    private readonly GCPTemplatesController _controller;
    private readonly Mock<IGCPTemplatesServices> _mockService;

    public GCPTemplatesControllerTests()
    {
        _mockService = new Mock<IGCPTemplatesServices>();
        _controller = new GCPTemplatesController(_mockService.Object);
    }


    [Fact]
    public async Task UploadTemplate_ReturnsBadRequest_WhenFileIsInvalid()
    {
        
        var result = await _controller.UploadTemplate(null!, "Template1", "Sample description");

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid archive, try another one", badRequestResult.Value);
    }

    [Fact]
    public async Task UploadTemplate_ReturnsBadRequest_OnException()
    {
        
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Throws(new Exception("Unexpected error"));

       
        var result = await _controller.UploadTemplate(fileMock.Object, "Template1", "Sample description");

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Unexpected error", badRequestResult.Value);
    }

    [Fact]
    public void AllTemplates_ReturnsOk_WithTemplateList()
    {
        
        var templates = new List<GCPTemplates>
    {
        new GCPTemplates { ID = 1, TEMPLATE_NAME = "Template1", DESCRIPTIONS = "Description1" },
        new GCPTemplates { ID = 2, TEMPLATE_NAME = "Template2", DESCRIPTIONS = "Description2" }
    };

        _mockService.Setup(s => s.AllTemplates()).Returns(templates);

        
        var result = _controller.AllTemplates();

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result); // Accede a Result en lugar de result directamente
        var returnedTemplates = Assert.IsType<List<GCPTemplates>>(okResult.Value);
        Assert.Equal(2, returnedTemplates.Count);
    }


    [Fact]
    public void AllTemplates_ReturnsBadRequest_OnException()
    {
        
        _mockService.Setup(s => s.AllTemplates()).Throws(new Exception("Database error"));

        
        var result = _controller.AllTemplates();

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // Accede a Result
        Assert.Equal("Database error", badRequestResult.Value);
    }


    [Fact]
    public async Task DeleteATemplateByID_ReturnsOk_WhenSuccessful()
    {
        
        int templateId = 1;
        _mockService.Setup(s => s.RemoveTemplate(templateId)).ReturnsAsync("Template deleted successfully");

        
        var result = await _controller.DeleteATemplateByID(templateId);

        
        var okResult = Assert.IsType<OkObjectResult>(result.Result); // Accede a Result
        Assert.Equal("Template deleted successfully", okResult.Value);
    }

    [Fact]
    public async Task DeleteATemplateByID_ReturnsBadRequest_OnException()
    {
        
        int templateId = 1;
        _mockService.Setup(s => s.RemoveTemplate(templateId)).Throws(new Exception("Deletion failed"));

       
        var result = await _controller.DeleteATemplateByID(templateId);

        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // Accede a Result
        Assert.Equal("Deletion failed", badRequestResult.Value);
    }

}
