using MROCoatching.Abstractions.SkyLight;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GroundServicePlanning.DataObjects.Utilities
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;
        private readonly IServiceProvider serviceProvider;

        public ViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model, IHostingEnvironment _environment)
        {
            try
            {
                var httpContext = new DefaultHttpContext { RequestServices = this.serviceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                using (var sw = new StringWriter())
                {
                    var viewResult = this.razorViewEngine.GetView(null, viewName, false);

                    if (viewResult == null || viewResult.View == null || !viewResult.Success)
                    {
                        WriteToFile($"Couldn't find the file in {viewName}", _environment.WebRootPath);
                        return null;
                    }

                    var viewDictionary =
                        new ViewDataDictionary(
                            new EmptyModelMetadataProvider(),
                            new ModelStateDictionary())
                        { Model = model };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, this.tempDataProvider),
                        sw,
                        new HtmlHelperOptions());

                    await viewResult.View.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString(), _environment.WebRootPath);
                return null;
            }
        }

        public void WriteToFile(string text, string rootPath)
        {
            try
            {
                string path = Path.Combine(rootPath, $"Logging\\");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = DateTime.Now.ToString("dd-MM-yyyy tt") + " SystemLog.txt";
                string filePath = path + $"{fileName}";
                StreamWriter erroLogWriter = new StreamWriter(filePath, true);
                erroLogWriter.WriteLine(text);
                erroLogWriter.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
