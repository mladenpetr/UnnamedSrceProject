using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrceApplicaton.Models
{

    public enum JobStates
    {
        Unassigned = 0,
        Assigned = 1,
        Locked = 2,
        Archived = 3,
    }

    public sealed class AccessLevel
    {
        public static readonly AccessLevel Technician = new AccessLevel("Technician");
        public static readonly AccessLevel Employer = new AccessLevel("Employer");

        public string Value { get; private set; }

        public AccessLevel(string value)
        {
            Value = value;
        }
    }

    public sealed class Colors
    {
        public static readonly Colors Red = new Colors("#dd0d0d");
        public static readonly Colors Blue = new Colors("#1417ce");
        public static readonly Colors Green = new Colors("#2ba81f");
        public static readonly Colors Yellow = new Colors("#deea72");
        public static readonly Colors Orange = new Colors("#f7a71d");
        public static readonly Colors Purple = new Colors("#ad1aa5");
        public static readonly Colors Cyan = new Colors("#27dddd");

        public string Value { get; private set; }

        public Colors(string value)
        {
            Value = value;
        }
    }
}