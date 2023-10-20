using Formation.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Formation.WPF
{
    public class ClientViewModel :CommandBase<ImagesListResponse>
    {
        private DemoEntities dbContext = new DemoEntities();

        // Default Docker Engine on Windows
        DockerClient client = new DockerClientConfiguration(
            new Uri("npipe://./pipe/docker_engine"))
             .CreateClient();

        /// <summary>
        /// Fetches all Docker Images
        /// </summary>
        /// <returns></returns>
        protected override async Task Get()
        {
            try
            {                                
                ImagesListParameters imageParams = new ImagesListParameters() { All = true };
                IList<ImagesListResponse> dockerImages = await client.Images.ListImagesAsync(imageParams);
                var imageCollection = new ObservableCollection<ImagesListResponse>();
                foreach (var item in dockerImages)
                {
                    imageCollection.Add(item);
                }
                Collection = imageCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        

        protected override bool CanGet()
        {
            return true;
        }
        protected override void Save()
        {
            //foreach (Client item in Collection)
            //{
            //    if (dbContext.Entry(item).State == System.Data.Entity.EntityState.Added)
            //    {
            //        dbContext.Client.Add(item);
            //    }
            //}
            //dbContext.SaveChanges();
        }
        protected override void Delete()
        {

        }
    }
}

