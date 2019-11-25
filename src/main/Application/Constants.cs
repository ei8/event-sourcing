using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.EventSourcing.Application
{
    public struct Constants
    {
        public struct Messages
        {
            public struct Exception
            {
                public const string AvatarIdRequired = "Avatar ID cannot be empty.";
            }
        }
    }
}
