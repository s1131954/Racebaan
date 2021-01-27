using System;
using System.Collections.Generic;
using System.Text;
using static Model.Section;

using static Model.Section;

namespace Model
{
    public class Track
    {
        public string Name;
        public LinkedList<Section> Sections;

        public Track(string name, SectionTypes[] sections)
        {
            Name = name;
            Sections = ConvertToLinkedList(sections);
        }

        public LinkedList<Section> ConvertToLinkedList (SectionTypes[] sections)
        {
            LinkedList<Section> sectionLinkedList = new LinkedList<Section>();
            foreach (var section in sections)
            {
                LinkedListNode<Section> sectionLinkedListNode = new LinkedListNode<Section>(new Section(section));
                sectionLinkedList.AddLast(sectionLinkedListNode);
            }
            return sectionLinkedList;
        }
    }
}
