﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Timetable.Models
{
    [DataContract]
    public class Lesson
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Room { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Teacher { get; set; }

        [DataMember]
        public string Type { get; set; }

        public Brush Color { get; set; }

        public Lesson() { }

        public Lesson(string number, string room, string name, string teacher, string type)
        {
            Number = number;
            Room = room;
            Name = name;
            Teacher = teacher;
            Type = type;
        }

        public Lesson(List<string> parameters)
        {
            Room = parameters[0];
            Number = parameters[1];
            Teacher = parameters[2].Trim().Length > 0 ? parameters[2] : "-";
            Type = parameters[3];
            Name = parameters[4];
        }

        public override string ToString()
        {
            return $"{Number}, {Name}, {Room}, {Teacher}, {Type}";
        }
    }
}
