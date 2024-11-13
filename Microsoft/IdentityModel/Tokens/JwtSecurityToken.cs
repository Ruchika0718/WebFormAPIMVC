﻿using System;
using System.Security.Claims;

namespace Microsoft.IdentityModel.Tokens
{
    internal class JwtSecurityToken
    {
        private string issuer;
        private string audience;
        private Claim[] claims;
        private DateTime expires;
        private SigningCredentials signingCredentials;

        public JwtSecurityToken(string issuer, string audience, Claim[] claims, DateTime expires, SigningCredentials signingCredentials)
        {
            this.issuer = issuer;
            this.audience = audience;
            this.claims = claims;
            this.expires = expires;
            this.signingCredentials = signingCredentials;
        }
    }
}