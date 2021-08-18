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
using OpenTidl.Core.Models;
using OpenTidl.Core.Models.Base;
using OpenTidl.Core.Transport;

namespace OpenTidl.Core.Methods
{
    public partial class OpenTidlSession
    {
        #region properties

        private OpenTidlClient OpenTidlClient { get; set; }
        //private RestClient RestClient { get { return OpenTidlClient.RestClient; } }
        private RestClient RestClient { get; set; }

        public LoginModel LoginResult { get; private set; }

        //FIXME: Throw error if empty
        public string SessionId { get { return LoginResult != null ? LoginResult.SessionId : null; } }
        public int UserId { get { return LoginResult != null ? LoginResult.UserId : 0; } }
        public string CountryCode { get { return LoginResult != null ? LoginResult.CountryCode : null; } }

        #endregion


        #region opentidl methods
        

        #region logout methods

        public async Task<EmptyModel> Logout()
        {
            var result = await RestClient.Process<EmptyModel>("/logout", new
            {
                sessionId = SessionId,
                countryCode = CountryCode
            }, new { }, "POST");

            if (result == null || result.Exception == null)
                this.LoginResult = null; //Clear session
            return HandleResponse(result);
        }

        #endregion


        #region playlist methods

        public async Task<PlaylistModel> GetPlaylist(string playlistUuid)
        {
            return HandleResponse(await RestClient.Process<PlaylistModel>(
                RestUtility.FormatUrl("/playlists/{uuid}", new { uuid = playlistUuid }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<JsonList<TrackModel>> GetPlaylistTracks(string playlistUuid, int offset = 0, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<TrackModel>>(
                RestUtility.FormatUrl("/playlists/{uuid}/tracks", new { uuid = playlistUuid }), new
                {
                    offset = offset,
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<EmptyModel> AddPlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> trackIds, int toIndex = 0)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/playlists/{uuid}/tracks", new { uuid = playlistUuid }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    trackIds = string.Join(",", trackIds),
                    toIndex = toIndex
                }, "POST",
                Header("If-None-Match", playlistETag)));
        }

        public async Task<EmptyModel> DeletePlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> indices)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/playlists/{uuid}/tracks/{indices}", new
                {
                    uuid = playlistUuid,
                    indices = string.Join(",", indices)
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE",
                Header("If-None-Match", playlistETag)));
        }

        public async Task<EmptyModel> DeletePlaylist(string playlistUuid, string playlistETag)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/playlists/{uuid}", new
                {
                    uuid = playlistUuid
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE",
                Header("If-None-Match", playlistETag)));
        }

        public async Task<EmptyModel> MovePlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> indices, int toIndex = 0)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/playlists/{uuid}/tracks/{indices}", new
                {
                    uuid = playlistUuid,
                    indices = string.Join(",", indices)
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    toIndex = toIndex
                }, "POST",
                Header("If-None-Match", playlistETag)));
        }

        public async Task<EmptyModel> UpdatePlaylist(string playlistUuid, string playlistETag, string title)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/playlists/{uuid}", new
                {
                    uuid = playlistUuid
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    title = title
                }, "POST",
                Header("If-None-Match", playlistETag)));
        }

        #endregion


        #region session methods

        public async Task<ClientModel> GetClient()
        {
            return HandleResponse(await RestClient.Process<ClientModel>(
                RestUtility.FormatUrl("/sessions/{sessionId}/client", new { sessionId = SessionId }),
                null, null, "GET"));
        }

        public async Task<SessionModel> GetSession()
        {
            return HandleResponse(await RestClient.Process<SessionModel>(
                RestUtility.FormatUrl("/sessions/{sessionId}", new { sessionId = SessionId }),
                null, null, "GET"));
        }

        #endregion


        #region track methods

        public async Task<StreamUrlModel> GetTrackStreamUrl(int trackId, SoundQuality soundQuality, string playlistUuid)
        {
            return HandleResponse(await RestClient.Process<StreamUrlModel>(
                RestUtility.FormatUrl("/tracks/{id}/streamUrl", new { id = trackId }), new
                {
                    soundQuality = soundQuality,
                    playlistUuid = playlistUuid,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<StreamUrlModel> GetTrackOfflineUrl(int trackId, SoundQuality soundQuality)
        {
            return HandleResponse(await RestClient.Process<StreamUrlModel>(
                RestUtility.FormatUrl("/tracks/{id}/offlineUrl", new { id = trackId }), new
                {
                    soundQuality = soundQuality,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        #endregion


        #region user methods

        public async Task<JsonList<ClientModel>> GetUserClients(ClientFilter filter, int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<ClientModel>>(
                RestUtility.FormatUrl("/users/{userId}/clients", new { userId = UserId }), new
                {
                    filter = filter.ToString(),
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<JsonList<PlaylistModel>> GetUserPlaylists(int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<PlaylistModel>>(
                RestUtility.FormatUrl("/users/{userId}/playlists", new { userId = UserId }), new
                {
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<PlaylistModel> CreateUserPlaylist(string title)
        {
            return HandleResponse(await RestClient.Process<PlaylistModel>(
                RestUtility.FormatUrl("/users/{userId}/playlists", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    title = title
                }, "POST"));
        }

        public async Task<UserSubscriptionModel> GetUserSubscription()
        {
            return HandleResponse(await RestClient.Process<UserSubscriptionModel>(
                RestUtility.FormatUrl("/users/{userId}/subscription", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<UserModel> GetUser()
        {
            return HandleResponse(await RestClient.Process<UserModel>(
                RestUtility.FormatUrl("/users/{userId}", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        #endregion


        #region user favorites methods

        public async Task<JsonList<JsonListItem<AlbumModel>>> GetFavoriteAlbums(int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<JsonListItem<AlbumModel>>>(
                RestUtility.FormatUrl("/users/{userId}/favorites/albums", new { userId = UserId }), new
                {
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<JsonList<JsonListItem<ArtistModel>>> GetFavoriteArtists(int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<JsonListItem<ArtistModel>>>(
                RestUtility.FormatUrl("/users/{userId}/favorites/artists", new { userId = UserId }), new
                {
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<JsonList<JsonListItem<PlaylistModel>>> GetFavoritePlaylists(int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<JsonListItem<PlaylistModel>>>(
                RestUtility.FormatUrl("/users/{userId}/favorites/playlists", new { userId = UserId }), new
                {
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<JsonList<JsonListItem<TrackModel>>> GetFavoriteTracks(int limit = 9999)
        {
            return HandleResponse(await RestClient.Process<JsonList<JsonListItem<TrackModel>>>(
                RestUtility.FormatUrl("/users/{userId}/favorites/tracks", new { userId = UserId }), new
                {
                    limit = limit,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<EmptyModel> AddFavoriteAlbum(int albumId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/albums", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new {
                    albumId = albumId
                }, "POST"));
        }

        public async Task<EmptyModel> AddFavoriteArtist(int artistId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/artists", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    artistId = artistId
                }, "POST"));
        }

        public async Task<EmptyModel> AddFavoritePlaylist(string playlistUuid)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/playlists", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    uuid = playlistUuid
                }, "POST"));
        }

        public async Task<EmptyModel> AddFavoriteTrack(int trackId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/tracks", new { userId = UserId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, new
                {
                    trackId = trackId
                }, "POST"));
        }

        public async Task<EmptyModel> RemoveFavoriteAlbum(int albumId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/albums/{albumId}", new 
                { 
                    userId = UserId,
                    albumId = albumId
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE"));
        }

        public async Task<EmptyModel> RemoveFavoriteArtist(int artistId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/artists/{artistId}", new
                {
                    userId = UserId,
                    artistId = artistId
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE"));
        }

        public async Task<EmptyModel> RemoveFavoritePlaylist(string playlistUuid)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/playlists/{uuid}", new
                {
                    userId = UserId,
                    uuid = playlistUuid
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE"));
        }

        public async Task<EmptyModel> RemoveFavoriteTrack(int trackId)
        {
            return HandleResponse(await RestClient.Process<EmptyModel>(
                RestUtility.FormatUrl("/users/{userId}/favorites/tracks/{trackId}", new
                {
                    userId = UserId,
                    trackId = trackId
                }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "DELETE"));
        }

        #endregion


        #region video methods

        public async Task<VideoModel> GetVideo(int videoId)
        {
            return HandleResponse(await RestClient.Process<VideoModel>(
                RestUtility.FormatUrl("/videos/{id}", new { id = videoId }), new
                {
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        public async Task<VideoStreamUrlModel> GetVideoStreamUrl(int videoId, VideoQuality videoQuality)
        {
            return HandleResponse(await RestClient.Process<VideoStreamUrlModel>(
                RestUtility.FormatUrl("/videos/{id}/streamurl", new { id = videoId }), new
                {
                    videoQuality = videoQuality,
                    sessionId = SessionId,
                    countryCode = CountryCode
                }, null, "GET"));
        }

        #endregion


        #endregion
        
        #region opentidl methods


        #region logout methods

        public EmptyModel Logout(int? timeout)
        {
            return HelperExtensions.Sync(() => this.Logout(), timeout);
        }

        #endregion


        #region playlist methods

        public PlaylistModel GetPlaylist(string playlistUuid, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetPlaylist(playlistUuid), timeout);
        }

        public JsonList<TrackModel> GetPlaylistTracks(string playlistUuid, int? offset, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetPlaylistTracks(playlistUuid, offset ?? 0, limit ?? 9999), timeout);
        }

        public EmptyModel AddPlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> trackIds, int? toIndex, int? timeout)
        {
            return HelperExtensions.Sync(() => this.AddPlaylistTracks(playlistUuid, playlistETag, trackIds, toIndex ?? 0), timeout);
        }

        public EmptyModel DeletePlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> indices, int? timeout)
        {
            return HelperExtensions.Sync(() => this.DeletePlaylistTracks(playlistUuid, playlistETag, indices), timeout);
        }

        public EmptyModel DeletePlaylist(string playlistUuid, string playlistETag, int? timeout)
        {
            return HelperExtensions.Sync(() => this.DeletePlaylist(playlistUuid, playlistETag), timeout);
        }

        public EmptyModel MovePlaylistTracks(string playlistUuid, string playlistETag, IEnumerable<int> indices, int? toIndex, int? timeout)
        {
            return HelperExtensions.Sync(() => this.MovePlaylistTracks(playlistUuid, playlistETag, indices, toIndex ?? 0), timeout);
        }

        public EmptyModel UpdatePlaylist(string playlistUuid, string playlistETag, string title, int? timeout)
        {
            return HelperExtensions.Sync(() => this.UpdatePlaylist(playlistUuid, playlistETag, title), timeout);
        }

        #endregion


        #region session methods

        public ClientModel GetClient(int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetClient(), timeout);
        }

        public SessionModel GetSession(int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetSession(), timeout);
        }

        #endregion


        #region track methods

        public StreamUrlModel GetTrackStreamUrl(int trackId, SoundQuality soundQuality, string playlistUuid, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetTrackStreamUrl(trackId, soundQuality, playlistUuid), timeout);
        }

        public StreamUrlModel GetTrackOfflineUrl(int trackId, SoundQuality soundQuality, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetTrackOfflineUrl(trackId, soundQuality), timeout);
        }

        #endregion


        #region user methods

        public JsonList<ClientModel> GetUserClients(ClientFilter filter, int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetUserClients(filter, limit ?? 9999), timeout);
        }

        public JsonList<PlaylistModel> GetUserPlaylists(int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetUserPlaylists(limit ?? 9999), timeout);
        }

        public PlaylistModel CreateUserPlaylist(string title, int? timeout)
        {
            return HelperExtensions.Sync(() => this.CreateUserPlaylist(title), timeout);
        }

        public UserSubscriptionModel GetUserSubscription(int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetUserSubscription(), timeout);
        }

        public UserModel GetUser(int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetUser(), timeout);
        }

        #endregion


        #region user favorites methods

        public JsonList<JsonListItem<AlbumModel>> GetFavoriteAlbums(int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetFavoriteAlbums(limit ?? 9999), timeout);
        }

        public JsonList<JsonListItem<ArtistModel>> GetFavoriteArtists(int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetFavoriteArtists(limit ?? 9999), timeout);
        }

        public JsonList<JsonListItem<PlaylistModel>> GetFavoritePlaylists(int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetFavoritePlaylists(limit ?? 9999), timeout);
        }

        public JsonList<JsonListItem<TrackModel>> GetFavoriteTracks(int? limit, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetFavoriteTracks(limit ?? 9999), timeout);
        }

        public EmptyModel AddFavoriteAlbum(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.AddFavoriteAlbum(albumId), timeout);
        }

        public EmptyModel AddFavoriteArtist(int artistId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.AddFavoriteArtist(artistId), timeout);
        }

        public EmptyModel AddFavoritePlaylist(string playlistUuid, int? timeout)
        {
            return HelperExtensions.Sync(() => this.AddFavoritePlaylist(playlistUuid), timeout);
        }

        public EmptyModel AddFavoriteTrack(int trackId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.AddFavoriteTrack(trackId), timeout);
        }

        public EmptyModel RemoveFavoriteAlbum(int albumId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RemoveFavoriteAlbum(albumId), timeout);
        }

        public EmptyModel RemoveFavoriteArtist(int artistId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RemoveFavoriteArtist(artistId), timeout);
        }

        public EmptyModel RemoveFavoritePlaylist(string playlistUuid, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RemoveFavoritePlaylist(playlistUuid), timeout);
        }

        public EmptyModel RemoveFavoriteTrack(int trackId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.RemoveFavoriteTrack(trackId), timeout);
        }

        #endregion


        #region video methods

        public VideoModel GetVideo(int videoId, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetVideo(videoId), timeout);
        }

        public VideoStreamUrlModel GetVideoStreamUrl(int videoId, VideoQuality videoQuality, int? timeout)
        {
            return HelperExtensions.Sync(() => this.GetVideoStreamUrl(videoId, videoQuality), timeout);
        }

        #endregion


        #endregion


        #region methods

        private T HandleResponse<T>(RestResponse<T> response) where T : ModelBase
        {
            return this.OpenTidlClient.HandleResponse(response);
        }

        private KeyValuePair<string, string> Header(string header, string value)
        {
            return this.OpenTidlClient.Header(header, value);
        }

        #endregion


        #region construction

        internal OpenTidlSession(OpenTidlClient client, LoginModel loginModel)
        {
            this.OpenTidlClient = client;
            this.LoginResult = loginModel;
            this.RestClient = new RestClient(client.Configuration.ApiEndpoint, client.Configuration.UserAgent, Header("X-Tidal-SessionId", loginModel?.SessionId ?? ""));
        }

        #endregion
    }
}
