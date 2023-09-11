using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;

namespace VideoApp.BL.Repositories
{ 

    public class VideoRepository : IVideoRepository
    {
        private readonly RwaMoviesContext _dbContext;

        public VideoRepository(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BLVideo> GetVideos()
        {
            var dalVideos = _dbContext.Videos.Include("Genre")
                                             .Include("VideoTags")
                                             .Include("VideoTags.Tag")
                                             .Include("Image");

            var blVideos = VideoMapper.MapToBl(dalVideos);

            blVideos = dalVideos.Select(video => new BLVideo
            {
                Id = video.Id,
                CreatedAt = video.CreatedAt,
                Name = video.Name,
                Description = video.Description,
                GenreId = video.GenreId,
                Genre = video.Genre.Name, // Naziv žanra
                TotalTime = GetTime(video.TotalSeconds),
                StreamingUrl = video.StreamingUrl,
                ImageId = video.ImageId,
                Image = video.Image,
                Tags = video.VideoTags.Select(tag => tag.Tag.Name).ToList()
            });
                     

            return blVideos;
        }

        static TimeSpan GetTime(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time;
        }

        public BLVideo GetVideo(int id)
        {
            var dalVideo = _dbContext.Videos.Include("Genre")
                                             .Include("VideoTags")
                                             .Include("VideoTags.Tag")
                                             .Include("Image")
                                             .FirstOrDefault(x => x.Id == id);

            var blVideo = VideoMapper.MapToBl(dalVideo);
            blVideo.Genre = dalVideo.Genre.Name;

            return blVideo;
        }

        public (IEnumerable<BLVideo>, int) GetPagedVideos(string? filter, int page, int size, string orderBy, string direction)
        {
            IEnumerable<Video> dalVideos = _dbContext.Videos.Include("Genre")
                                                            .Include("VideoTags")
                                                            .Include("VideoTags.Tag")
                                                            .Include("Image")
                                                            .AsEnumerable();

            if(filter != null)
            {
               dalVideos = dalVideos.ToList().
               Where(x => x.Name.Contains(filter)).ToList();
            }
                        

            //sortiranje
            if (string.Compare(orderBy, "id", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.TotalSeconds);
            }
            else // default: order by Id
            {
                dalVideos = dalVideos.OrderBy(x => x.Id);
            }

            //desc
            if (string.Compare(direction, "desc", true) == 0)
            {
                dalVideos = dalVideos.Reverse();
            }

            var unpagedCount = dalVideos.Count();

            //page
            if(page != null && size != null)
            {
                dalVideos = dalVideos.Skip(page * size).Take(size);
            }           

            var blVideos = VideoMapper.MapToBl(dalVideos);

            return (blVideos, unpagedCount);
        }

        public (IEnumerable<BLVideo>, int) GetPagedVideosAdmin(string? filterName, string? filterGenre, int page, int size, string orderBy, string direction)
        {
            IEnumerable<Video> dalVideos = _dbContext.Videos.Include("Genre")
                                                            .Include("VideoTags")
                                                            .Include("VideoTags.Tag")
                                                            .Include("Image")
                                                            .AsEnumerable();

            if (filterName != null)
            {
                dalVideos = dalVideos.ToList().
                Where(x => x.Name.Contains(filterName)).ToList();
            }

            if (filterGenre != null)
            {
                dalVideos = dalVideos.ToList().
                Where(x => x.Genre.Name.Contains(filterGenre)).ToList();
            }


            //sortiranje
            if (string.Compare(orderBy, "id", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.Name);
            }
            else if (string.Compare(orderBy, "description", ignoreCase: true) == 0)
            {
                dalVideos = dalVideos.OrderBy(x => x.TotalSeconds);
            }
            else // default: order by Id
            {
                dalVideos = dalVideos.OrderBy(x => x.Id);
            }

            //desc
            if (string.Compare(direction, "desc", true) == 0)
            {
                dalVideos = dalVideos.Reverse();
            }

            var unpagedCount = dalVideos.Count();

            //page
            dalVideos = dalVideos.Skip(page * size).Take(size);



            var blVideos = VideoMapper.MapToBl(dalVideos);

           blVideos = dalVideos.Select(video => new BLVideo
            {
                Id = video.Id,
                CreatedAt = video.CreatedAt,
                Name = video.Name,
                Description = video.Description,
                GenreId = video.GenreId,
                Genre = video.Genre.Name, // Naziv žanra
                TotalTime = GetTime(video.TotalSeconds),
                StreamingUrl = video.StreamingUrl,
                ImageId = video.ImageId,
                Image = video.Image,
                Tags = video.VideoTags.Select(tag => tag.Tag.Name).ToList()
            });

            TimeSpan GetTime(int seconds)
            {
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                return time;
            }

            return (blVideos, unpagedCount);
        }

        public int GetTotal() => _dbContext.Videos.Count();

    }
}
