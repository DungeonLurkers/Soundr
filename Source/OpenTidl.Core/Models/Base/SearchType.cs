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

namespace OpenTidl.Core.Models.Base
{
    public class SearchType
    {
        #region properties

        public bool Albums { get; private set; }
        public bool Artists { get; private set; }
        public bool Playlists { get; private set; }
        public bool Tracks { get; private set; }
        public bool Videos { get; private set; }

        #endregion


        #region methods

        public override string ToString()
        {
            var types = new List<string>();
            if (this.Albums) types.Add("ALBUMS");
            if (this.Artists) types.Add("ARTISTS");
            if (this.Playlists) types.Add("PLAYLISTS");
            if (this.Tracks) types.Add("TRACKS");
            if (this.Videos) types.Add("VIDEOS");
            return string.Join(",", types);
        }

        #endregion


        #region construction

        public static SearchType ALL
        {
            get
            {
                return new SearchType(true, true, true, true, true);
            }
        }

        public static SearchType ALBUMS
        {
            get
            {
                return new SearchType(true, false, false, false, false);
            }
        }

        public static SearchType ARTISTS
        {
            get
            {
                return new SearchType(false, true, false, false, false);
            }
        }

        public static SearchType PLAYLISTS
        {
            get
            {
                return new SearchType(false, false, true, false, false);
            }
        }

        public static SearchType TRACKS
        {
            get
            {
                return new SearchType(false, false, false, true, false);
            }
        }

        public static SearchType VIDEOS
        {
            get
            {
                return new SearchType(false, false, false, false, true);
            }
        }

        public static SearchType Select(bool albums, bool artists, bool playlists, bool tracks, bool videos)
        {
            return new SearchType(albums, artists, playlists, tracks, videos);
        }

        private SearchType(bool albums, bool artists, bool playlists, bool tracks, bool videos)
        {
            this.Albums = albums;
            this.Artists = artists;
            this.Playlists = playlists;
            this.Tracks = tracks;
            this.Videos = videos;
        }

        #endregion
    }
}
