using Entities.User;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dtos
{
	public class ActiveSessionDto
	{
		public int Id { get; set; }
		public int? CreatorUserId { get; set; }

        public string CreatorIP { get; set; }

        public string DeviceName { get; set; }

        public DateTime LoginDate { get; set; }

        public OSType OSType { get; set; }

        public BrowserType BrowserType { get; set; }

        public DeviceType DeviceType { get; set; }

        public bool IsCurrent { get; set; }
    }
}
