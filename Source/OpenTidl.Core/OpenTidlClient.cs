/*
    Copyright (C) 2015  Jack Fagner

    This file is part of OpenTidl.

    OpenTidl is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    OpenTidl is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with OpenTidl.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTidl.Core.Enums;
using OpenTidl.Core.Methods;
using OpenTidl.Core.Models;
using OpenTidl.Core.Models.Base;
using OpenTidl.Core.Transport;

namespace OpenTidl.Core
{
    public partial class OpenTidlClient
    {
        #region fields

        private const string FALLBACK_COUNTRY_CODE = "US";
        private Lazy<string> _defaultCountryCode;

        #endregion


        #region properties

        public ClientConfiguration Configuration { get; private set; }
        private RestClient RestClient { get; set; }

        private string LastSessionCountryCode { get; set; }
        private string DefaultCountryCode { get { return _defaultCountryCode.Value; } }

        #endregion


        #region methods
        
        internal KeyValuePair<string, string> Header(string header, string value)
        {
            return new KeyValuePair<string, string>(header, value);
        }

        internal T HandleResponse<T>(RestResponse<T> response) where T : ModelBase
        {
            if (response.Exception != null)
                throw response.Exception;
            return response.Model;
        }

        private LoginModel HandleLoginResponse(RestResponse<LoginModel> response, LoginType loginType)
        {
            if (response.Exception != null)
                throw response.Exception;
            var model = response.Model;
            if (model != null)
            {
                this.LastSessionCountryCode = model.CountryCode;
                model.LoginType = loginType;
            }
            return model;
        }

        private SessionModel HandleSessionResponse(RestResponse<SessionModel> response)
        {
            if (response.Exception != null)
                throw response.Exception;
            var model = response.Model;
            if (model != null)
                this.LastSessionCountryCode = model.CountryCode;
            return model;
        }

        private string GetCountryCode()
        {
            return !string.IsNullOrEmpty(LastSessionCountryCode) ? LastSessionCountryCode : DefaultCountryCode;
        }

        private string GetDefaultCountryCode()
        {
            CountryModel cc = null;
            try
            {
                cc = this.GetCountry(1000);
            }
            catch { }
            if (cc != null && !string.IsNullOrEmpty(cc.CountryCode))
                return cc.CountryCode;
            if (Configuration != null && !string.IsNullOrEmpty(Configuration.DefaultCountryCode))
                return Configuration.DefaultCountryCode;
            return FALLBACK_COUNTRY_CODE;
        }

        #endregion


        #region construction

        public OpenTidlClient(ClientConfiguration config)
        {
            this.Configuration = config;
            this.RestClient = new RestClient(config.ApiEndpoint, config.UserAgent, Header("X-Tidal-Token", config.Token));
            this._defaultCountryCode = new Lazy<string>(() => GetDefaultCountryCode());
        }

        #endregion
        
        #region login methods

        public async Task<OpenTidlSession> LoginWithFacebook(string accessToken)
        {
            return new OpenTidlSession(this, HandleLoginResponse(await RestClient.Process<LoginModel>("/login/facebook", null, new
            {
                accessToken = accessToken,
                token = Configuration.Token,
                clientUniqueKey = Configuration.ClientUniqueKey,
                clientVersion = Configuration.ClientVersion
            }, "POST"), LoginType.Facebook));
        }

        /*[Obsolete]
        public async Task<OpenTidlSession> LoginWithSpidToken(String accessToken, String spidUserId)
        {
            return new OpenTidlSession(this, HandleLoginResponse(await RestClient.Process<LoginModel>("/login/spid/token", null, new
            {
                accessToken = accessToken,
                spidUserId = spidUserId,
                token = Configuration.Token,
                clientUniqueKey = Configuration.ClientUniqueKey,
                clientVersion = Configuration.ClientVersion
            }, "POST"), LoginType.Spid));
        }*/

        public async Task<OpenTidlSession> LoginWithToken(string authenticationToken)
        {
            return new OpenTidlSession(this, HandleLoginResponse(await RestClient.Process<LoginModel>("/login/token", null, new
            {
                authenticationToken = authenticationToken,
                token = Configuration.Token,
                clientUniqueKey = Configuration.ClientUniqueKey,
                clientVersion = Configuration.ClientVersion
            }, "POST"), LoginType.Token));
        }

        public async Task<OpenTidlSession> LoginWithTwitter(string accessToken, string accessTokenSecret)
        {
            return new OpenTidlSession(this, HandleLoginResponse(await RestClient.Process<LoginModel>("/login/twitter", null, new
            {
                accessToken = accessToken,
                accessTokenSecret = accessTokenSecret,
                token = Configuration.Token,
                clientUniqueKey = Configuration.ClientUniqueKey,
                clientVersion = Configuration.ClientVersion
            }, "POST"), LoginType.Twitter));
        }

        public async Task<OpenTidlSession> LoginWithUsername(string username, string password)
        {
            return new OpenTidlSession(this, HandleLoginResponse(await RestClient.Process<LoginModel>("/login/username", null, new
            {
                username = username,
                password = password,
                token = Configuration.Token,
                clientUniqueKey = Configuration.ClientUniqueKey,
                clientVersion = Configuration.ClientVersion
            }, "POST"), LoginType.Username));
        }

        #endregion


        #region session methods

        public async Task<OpenTidlSession> RestoreSession(string sessionId)
        {
            return new OpenTidlSession(this, LoginModel.FromSession(HandleSessionResponse(await RestClient.Process<SessionModel>(
                RestUtility.FormatUrl("/sessions/{sessionId}", new { sessionId = sessionId }),
                null, null, "GET"))));
        }

        #endregion


        #region user methods

        public async Task<RecoverPasswordResponseModel> RecoverPassword(string username)
        {
            return HandleResponse(await RestClient.Process<RecoverPasswordResponseModel>(
                RestUtility.FormatUrl("/users/{username}/recoverpassword", new { username = username }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }
        
        #endregion
        
        #region login methods

        public OpenTidlSession LoginWithFacebook(string accessToken, int? timeout)
        {
            return HelperExtensions.Sync(() => this.LoginWithFacebook(accessToken), timeout);
        }

        /*[Obsolete]
        public OpenTidlSession LoginWithSpidToken(String accessToken, String spidUserId, Int32? timeout)
        {
            return HelperExtensions.Sync(() => this.LoginWithSpidToken(accessToken, spidUserId), timeout);
        }*/

        public OpenTidlSession LoginWithToken(string authenticationToken, int? timeout)
        {
            return HelperExtensions.Sync(() => this.LoginWithToken(authenticationToken), timeout);
        }

        public OpenTidlSession LoginWithTwitter(string accessToken, string accessTokenSecret, int? timeout)
        {
            return HelperExtensions.Sync(() => this.LoginWithTwitter(accessToken, accessTokenSecret), timeout);
        }

        public OpenTidlSession LoginWithUsername(string username, string password, int? timeout)
        {
            return HelperExtensions.Sync(() => this.LoginWithUsername(username, password), timeout);
        }

        #endregion


        #region session methods

        public OpenTidlSession RestoreSession(string sessionId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RestoreSession(sessionId), timeout);
        }

        #endregion


        #region user methods

        public RecoverPasswordResponseModel RecoverPassword(string username, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RecoverPassword(username), timeout);
        }

        #endregion
        
        #region album methods

        public async Task<AlbumModel> GetAlbum(int albumId)
        {
            return HandleResponse(await RestClient.Process<AlbumModel>(
                RestUtility.FormatUrl("/albums/{id}", new { id = albumId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<ModelArray<AlbumModel>> GetAlbums(IEnumerable<int> albumIds)
        {
            return HandleResponse(await RestClient.Process<ModelArray<AlbumModel>>(
                "/albums", new
                {
                    ids = string.Join(",", albumIds),
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<AlbumModel>> GetSimilarAlbums(int albumId)
        {
            return HandleResponse(await RestClient.Process<JsonList<AlbumModel>>(
                RestUtility.FormatUrl("/albums/{id}/similar", new { id = albumId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> GetAlbumTracks(int albumId)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                RestUtility.FormatUrl("/albums/{id}/tracks", new { id = albumId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<AlbumReviewModel> GetAlbumReview(int albumId)
        {
            return HandleResponse(await RestClient.Process<AlbumReviewModel>(
                RestUtility.FormatUrl("/albums/{id}/review", new { id = albumId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        #endregion


        #region artist methods

        public async Task<ArtistModel> GetArtist(int artistId)
        {
            return HandleResponse(await RestClient.Process<ArtistModel>(
                RestUtility.FormatUrl("/artists/{id}", new { id = artistId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<AlbumModel>> GetArtistAlbums(int artistId, AlbumFilter filter, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<AlbumModel>>(
                RestUtility.FormatUrl("/artists/{id}/albums", new { id = artistId }), new
                {
                    filter = filter.ToString(),
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> GetRadioFromArtist(int artistId, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                RestUtility.FormatUrl("/artists/{id}/radio", new { id = artistId }), new
                {
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<ArtistModel>> GetSimilarArtists(int artistId, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<ArtistModel>>(
                RestUtility.FormatUrl("/artists/{id}/similar", new { id = artistId }), new
                {
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> GetArtistTopTracks(int artistId, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                RestUtility.FormatUrl("/artists/{id}/toptracks", new { id = artistId }), new
                {
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<VideoModel>> GetArtistVideos(int artistId, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<VideoModel>>(
                RestUtility.FormatUrl("/artists/{id}/videos", new { id = artistId }), new
                {
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<ArtistBiographyModel> GetArtistBiography(int artistId)
        {
            return HandleResponse(await RestClient.Process<ArtistBiographyModel>(
                RestUtility.FormatUrl("/artists/{id}/bio", new { id = artistId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<LinkModel>> GetArtistLinks(int artistId, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<LinkModel>>(
                RestUtility.FormatUrl("/artists/{id}/links", new { id = artistId }), new
                {
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        #endregion


        #region country methods

        public async Task<CountryModel> GetCountry()
        {
            return HandleResponse(await RestClient.Process<CountryModel>("/country", null, null, "GET"));
        }

        #endregion

        
        #region search methods

        public async Task<JsonList<AlbumModel>> SearchAlbums(string query, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<AlbumModel>>(
                "/search/albums", new
                {
                    query = query,
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<ArtistModel>> SearchArtists(string query, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<ArtistModel>>(
                "/search/artists", new
                {
                    query = query,
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<PlaylistModel>> SearchPlaylists(string query, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<PlaylistModel>>(
                "/search/playlists", new
                {
                    query = query,
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> SearchTracks(string query, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                "/search/tracks", new
                {
                    query = query,
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<VideoModel>> SearchVideos(string query, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<VideoModel>>(
                "/search/videos", new
                {
                    query = query,
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<SearchResultModel> Search(string query, SearchType types, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<SearchResultModel>(
                "/search", new
                {
                    query = query,
                    types = types.ToString(),
                    offset = offset,
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        #endregion


        #region track methods

        public async Task<TrackModel> GetTrack(int trackId)
        {
            return HandleResponse(await RestClient.Process<TrackModel>(
                RestUtility.FormatUrl("/tracks/{id}", new { id = trackId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<ContributorModel>> GetTrackContributors(int trackId)
        {
            return HandleResponse(await RestClient.Process<JsonList<ContributorModel>>(
                RestUtility.FormatUrl("/tracks/{id}/contributors", new { id = trackId }), new
                {
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> GetRadioFromTrack(int trackId, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                RestUtility.FormatUrl("/tracks/{id}/radio", new { id = trackId }), new
                {
                    limit = limit,
                    token = Configuration.Token,
                    countryCode = GetCountryCode()
                }, null, "GET"));
        }

        #endregion
        
        #region album methods

        public AlbumModel GetAlbum(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetAlbum(albumId), timeout);
        }

        public ModelArray<AlbumModel> GetAlbums(IEnumerable<int> albumIds, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetAlbums(albumIds), timeout);
        }

        public JsonList<AlbumModel> GetSimilarAlbums(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetSimilarAlbums(albumId), timeout);
        }

        public JsonList<TrackModel> GetAlbumTracks(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetAlbumTracks(albumId), timeout);
        }

        public AlbumReviewModel GetAlbumReview(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetAlbumReview(albumId), timeout);
        }

        #endregion


        #region artist methods

        public ArtistModel GetArtist(int artistId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtist(artistId), timeout);
        }

        public JsonList<AlbumModel> GetArtistAlbums(int artistId, AlbumFilter filter, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtistAlbums(artistId, filter, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<TrackModel> GetRadioFromArtist(int artistId, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetRadioFromArtist(artistId, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<ArtistModel> GetSimilarArtists(int artistId, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetSimilarArtists(artistId, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<TrackModel> GetArtistTopTracks(int artistId, int? offset, int? limit , int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtistTopTracks(artistId, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<VideoModel> GetArtistVideos(int artistId, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtistVideos(artistId, offset ?? 0, limit ?? 9999), timeout);
        }

        public ArtistBiographyModel GetArtistBiography(int artistId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtistBiography(artistId), timeout);
        }

        public JsonList<LinkModel> GetArtistLinks(int artistId, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetArtistLinks(artistId, limit ?? 9999), timeout);
        }

        #endregion


        #region country methods

        public CountryModel GetCountry(int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetCountry(), timeout);
        }

        #endregion


        #region search methods

        public JsonList<AlbumModel> SearchAlbums(string query, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.SearchAlbums(query, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<ArtistModel> SearchArtists(string query, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.SearchArtists(query, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<PlaylistModel> SearchPlaylists(string query, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.SearchPlaylists(query, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<TrackModel> SearchTracks(string query, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.SearchTracks(query, offset ?? 0, limit ?? 9999), timeout);
        }

        public JsonList<VideoModel> SearchVideos(string query, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.SearchVideos(query, offset ?? 0, limit ?? 9999), timeout);
        }

        public SearchResultModel Search(string query, SearchType types, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.Search(query, types, offset ?? 0, limit ?? 9999), timeout);
        }

        #endregion


        #region track methods

        public TrackModel GetTrack(int trackId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetTrack(trackId), timeout);
        }

        public JsonList<ContributorModel> GetTrackContributors(int trackId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetTrackContributors(trackId), timeout);
        }

        public JsonList<TrackModel> GetRadioFromTrack(int trackId, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetRadioFromTrack(trackId, limit ?? 9999), timeout);
        }

        #endregion
        
        #region image methods

        /// <summary>
        /// Helper method to retrieve a stream with an album cover image
        /// </summary>
        public WebStreamModel GetAlbumCover(AlbumModel model, AlbumCoverSize size)
        {
            return GetAlbumCover(model.Cover, model.Id, size);
        }

        /// <summary>
        /// Helper method to retrieve a stream with an album cover image
        /// </summary>
        public WebStreamModel GetAlbumCover(string cover, int albumId, AlbumCoverSize size)
        {
            var w = 750;
            var h = 750;
            if (!RestUtility.ParseImageSize(size.ToString(), out w, out h))
                throw new ArgumentException("Invalid image size", "size");
            string url = null;
            if (!string.IsNullOrEmpty(cover))
                url = string.Format("http://resources.wimpmusic.com/images/{0}/{1}x{2}.jpg", cover.Replace('-', '/'), w, h);
            else
                url = string.Format("http://images.tidalhifi.com/im/im?w={1}&h={2}&albumid={0}&noph", albumId, w, h);
            return new WebStreamModel(RestClient.GetWebResponse(url));
        }

        /// <summary>
        /// Helper method to retrieve a stream with an artists picture
        /// </summary>
        public WebStreamModel GetArtistPicture(ArtistModel model, ArtistPictureSize size)
        {
            return GetArtistPicture(model.Picture, model.Id, size);
        }

        /// <summary>
        /// Helper method to retrieve a stream with an artists picture
        /// </summary>
        public WebStreamModel GetArtistPicture(string picture, int artistId, ArtistPictureSize size)
        {
            var w = 750;
            var h = 500;
            if (!RestUtility.ParseImageSize(size.ToString(), out w, out h))
                throw new ArgumentException("Invalid image size", "size");
            string url = null;
            if (!string.IsNullOrEmpty(picture))
                url = string.Format("http://resources.wimpmusic.com/images/{0}/{1}x{2}.jpg", picture.Replace('-', '/'), w, h);
            else
                url = string.Format("http://images.tidalhifi.com/im/im?w={1}&h={2}&artistid={0}&noph", artistId, w, h);
            return new WebStreamModel(RestClient.GetWebResponse(url));
        }

        /// <summary>
        /// Helper method to retrieve a stream with a playlist image
        /// </summary>
        public WebStreamModel GetPlaylistImage(PlaylistModel model, PlaylistImageSize size)
        {
            return GetPlaylistImage(model.Image, model.Uuid, size);
        }

        /// <summary>
        /// Helper method to retrieve a stream with a playlist image
        /// </summary>
        public WebStreamModel GetPlaylistImage(string image, string playlistUuid, PlaylistImageSize size)
        {
            var w = 750;
            var h = 500;
            if (!RestUtility.ParseImageSize(size.ToString(), out w, out h))
                throw new ArgumentException("Invalid image size", "size");
            string url = null;
            if (!string.IsNullOrEmpty(image))
                url = string.Format("http://resources.wimpmusic.com/images/{0}/{1}x{2}.jpg", image.Replace('-', '/'), w, h);
            else
                url = string.Format("http://images.tidalhifi.com/im/im?w={1}&h={2}&uuid={0}&rows=2&cols=3&noph", playlistUuid, w, h);
            return new WebStreamModel(RestClient.GetWebResponse(url));
        }

        /// <summary>
        /// Helper method to retrieve a stream with a video conver image
        /// </summary>
        public WebStreamModel GetVideoImage(VideoModel model, VideoImageSize size)
        {
            return GetVideoImage(model.ImageId, model.ImagePath, size);
        }

        /// <summary>
        /// Helper method to retrieve a stream with a video conver image
        /// </summary>
        public WebStreamModel GetVideoImage(string imageId, string imagePath, VideoImageSize size)
        {
            var w = 750;
            var h = 500;
            if (!RestUtility.ParseImageSize(size.ToString(), out w, out h))
                throw new ArgumentException("Invalid image size", "size");
            string url = null;
            if (!string.IsNullOrEmpty(imageId))
                url = string.Format("http://resources.wimpmusic.com/images/{0}/{1}x{2}.jpg", imageId.Replace('-', '/'), w, h);
            else
                url = string.Format("http://images.tidalhifi.com/im/im?w={1}&h={2}&img={0}&noph", imagePath, w, h);
            return new WebStreamModel(RestClient.GetWebResponse(url));
        }

        #endregion


        #region track/video methods

        /// <summary>
        /// Helper method to retrieve the audio/video stream with correct user-agent, etc.
        /// </summary>
        public WebStreamModel GetWebStream(string streamUrl)
        {
            return new WebStreamModel(RestClient.GetWebResponse(streamUrl));
        }

        #endregion
    }
}
