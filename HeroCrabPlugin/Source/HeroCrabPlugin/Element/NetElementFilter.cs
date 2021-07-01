/* Copyright (c) Jeremy Buck "Jarmo" - HeroCrab Ltd. (https://github.com/herocrab)
Distributed under the MIT license. See the LICENSE.md file in the project root for more information. */
using HeroCrabPlugin.Stream;

namespace HeroCrabPlugin.Element
{
    /// <summary>
    /// Network element filter used when sending elements to network sessions.
    /// </summary>
    public class NetElementFilter
    {
        /// <summary>
        /// Network stream group; used in filtering.
        /// </summary>
        public NetStreamGroup StreamGroup { get; set; }

        /// <summary>
        /// Network recipient id; 0 = Everyone, 1+ = unique single recipient.
        /// </summary>
        public uint Recipient { get; set; } // 0 = Everyone

        /// <summary>
        /// Network excluded recipient id.
        /// </summary>
        public uint Exclude { get; set; }

        /// <summary>
        /// Network element filter used when sending elements to network sessions.
        /// </summary>
        public NetElementFilter()
        {
            StreamGroup = NetStreamGroup.Default;
            Recipient = 0;
        }

        /// <summary>
        /// Checks to see if this filter contains an element stream group.
        /// </summary>
        /// <param name="streamGroup"></param>
        /// <returns></returns>
        public bool Contains(NetStreamGroup streamGroup)
        {
            return (StreamGroup & streamGroup) == streamGroup;
        }
    }
}
