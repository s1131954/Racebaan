using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Section
    {
        public SectionTypes SectionType { get; set; }

        public enum SectionTypes
        {
            Straight, LeftCorner, RightCorner, StartGrid, FinishGrid
        }

        public Section(SectionTypes sectionType)
        {
            SectionType = sectionType;
        }
    }
}
