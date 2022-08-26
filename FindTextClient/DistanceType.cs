// <copyright file="FindText.cs" company="Keith Martin, FeiYue">
// Copyright (c) Keith Martin, FeiYue
// Licensed under the Apache License, Version 2.0 (the "License")</copyright>

using System;


namespace FindTextClient
{
    /// <summary>
    /// Custom class to allow sorting by distance for SearchResult
    /// </summary>
    public class DistanceType : IEquatable<DistanceType>, IComparable<DistanceType>
    {
        /// <summary>
        /// Creator with the 2 internals being set
        /// </summary>
        /// <param name="point"></param>
        /// <param name="distance"></param>
        public DistanceType(SearchResult point, double distance)
        {
            this.Point = point;
            this.Distance = distance;
        }

        /// <summary>
        /// The item that we wanted to sort
        /// </summary>
        public SearchResult Point { get; set; }
        
        /// <summary>
        /// What we are sorting by
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Am I equal to another object (only if I am the right type, and if so go call the other Equals)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is DistanceType objAsDistanceType)) return false;
            else return Equals(objAsDistanceType);
        }

        /// <summary>
        /// Am I equal to another instance of this class?
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DistanceType? other)
        {
            if (other == null) return false;
            return (this.Distance.Equals(other.Distance));
        }

        // Default comparer for DistanceType type.
        public int CompareTo(DistanceType? compareDistanceType)
        {
            // A null value means that this object is greater.
            if (compareDistanceType == null)
                return 1;

            else
                return this.Distance.CompareTo(compareDistanceType.Distance);
        }

        /// <summary>
        /// Use the distance as a Hash Code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)Math.Round(Distance);
        }

    }
}
