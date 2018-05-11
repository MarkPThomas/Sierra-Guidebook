// ***********************************************************************
// Assembly         : DB-Location-XReferencer.Core
// Author           : Mark Thomas
// Created          : 10-11-2017
//
// Last Modified By : Mark Thomas
// Last Modified On : 10-11-2017
// ***********************************************************************
// <copyright file="PeakbaggerLocationRaw.cs" company="">
//     Copyright ©  2017
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace DB_Location_XReferencer.Core
{
    /// <summary>
    /// Data object for the raw Peakbagger Location as scraped from the website.
    /// </summary>
    public class PeakbaggerLocation
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the other.
        /// </summary>
        /// <value>The name of the other.</value>
        public string OtherName { get; set; }
        /// <summary>
        /// Gets or sets the elevation.
        /// </summary>
        /// <value>The elevation.</value>
        public int? Elevation { get; set; }
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
        public double? Latitude { get; set; }
        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
        public double? Longitude { get; set; }
        /// <summary>
        /// Gets or sets the type of the peak.
        /// </summary>
        /// <value>The type of the peak.</value>
        public string PeakType { get; set; }
        /// <summary>
        /// Gets or sets the land.
        /// </summary>
        /// <value>The land.</value>
        public string Land { get; set; }
        /// <summary>
        /// Gets or sets the wilderness special area.
        /// </summary>
        /// <value>The wilderness special area.</value>
        public string Wilderness_SpecialArea { get; set; }
        /// <summary>
        /// Gets or sets the state province1.
        /// </summary>
        /// <value>The state province1.</value>
        public string State_Province1 { get; set; }
        /// <summary>
        /// Gets or sets the state province2.
        /// </summary>
        /// <value>The state province2.</value>
        public string State_Province2 { get; set; }
        /// <summary>
        /// Gets or sets the range1.
        /// </summary>
        /// <value>The range1.</value>
        public string Range1 { get; set; }
        /// <summary>
        /// Gets or sets the range2.
        /// </summary>
        /// <value>The range2.</value>
        public string Range2 { get; set; }
        /// <summary>
        /// Gets or sets the range3.
        /// </summary>
        /// <value>The range3.</value>
        public string Range3 { get; set; }
        /// <summary>
        /// Gets or sets the range4.
        /// </summary>
        /// <value>The range4.</value>
        public string Range4 { get; set; }
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string URL { get; set; }
    }
}
