using System;
using System.Collections.Generic;
using System.Linq;

namespace Alemow
{
    public class ProfileMatcher : IProfileMatcher
    {
        private readonly StringComparer _profileComparer = StringComparer.InvariantCultureIgnoreCase;

        private readonly IList<string> _profiles;

        public ProfileMatcher(IList<string> profiles)
        {
            _profiles = profiles;
        }

        public bool Matches(string profile)
        {
            return _profiles.Any(it => _profileComparer.Equals(it, profile));
        }
    }

    public interface IProfileMatcher
    {
        bool Matches(string profile);
    }
}