﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace SecureHttpClient.Test
{
    public class SslTest
    {
        [Fact]
        public async Task SslTest_ExpiredCertificate()
        {
            const string page = @"https://expired.badssl.com/";
            var expectedExceptions = new List<string> {"Javax.Net.Ssl.SSLHandshakeException", "System.Net.WebException"};
            var throwsExpectedException = false;
            try
            {
                await GetPageAsync(page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().ToString();
                throwsExpectedException = expectedExceptions.Contains(exceptionType);
            }
            Assert.True(throwsExpectedException);
        }

        [Fact]
        public async Task SslTest_WrongHostCertificate()
        {
            const string page = @"https://wrong.host.badssl.com/";
            var expectedExceptions = new List<string> { "Javax.Net.Ssl.SSLPeerUnverifiedException", "System.Net.WebException" };
            var throwsExpectedException = false;
            try
            {
                await GetPageAsync(page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().ToString();
                throwsExpectedException = expectedExceptions.Contains(exceptionType);
            }
            Assert.True(throwsExpectedException);
        }

        [Fact]
        public async Task SslTest_SelfSignedCertificate()
        {
            const string page = @"https://self-signed.badssl.com/";
            var expectedExceptions = new List<string> { "Javax.Net.Ssl.SSLHandshakeException", "System.Net.WebException" };
            var throwsExpectedException = false;
            try
            {
                await GetPageAsync(page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().ToString();
                throwsExpectedException = expectedExceptions.Contains(exceptionType);
            }
            Assert.True(throwsExpectedException);
        }

        [Fact]
        public async Task SslTest_UntrustedRootCertificate()
        {
            const string page = @"https://untrusted-root.badssl.com/";
            var expectedExceptions = new List<string> { "Javax.Net.Ssl.SSLHandshakeException", "System.Net.WebException" };
            var throwsExpectedException = false;
            try
            {
                await GetPageAsync(page).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().ToString();
                throwsExpectedException = expectedExceptions.Contains(exceptionType);
            }
            Assert.True(throwsExpectedException);
        }

        [Fact]
        public async Task SslTest_Sha256Certificate()
        {
            const string page = @"https://sha256.badssl.com/";
            await GetPageAsync(page).ConfigureAwait(false);
            Assert.True(true);
        }

        [Fact]
        public async Task SslTest_SubjectAltName()
        {
            const string page = @"https://www.prive.livretzesto.fr/";
            await GetPageAsync(page).ConfigureAwait(false);
            Assert.True(true);
        }

        [Fact]
        public async Task SslTest_HowsMySsl()
        {
            const string expectedTlsVersion = "TLS 1.2";
            const string expectedRating = "[Probably Okay|Improvable]";

            const string page = @"https://www.howsmyssl.com/a/check";
            var result = await GetPageAsync(page).ConfigureAwait(false);

            var json = JToken.Parse(result);
            var actualTlsVersion = json["tls_version"].ToString();
            var actualRating = json["rating"].ToString();

            Assert.Equal(expectedTlsVersion, actualTlsVersion);
            Assert.Matches(expectedRating, actualRating);
        }

        private static async Task<string> GetPageAsync(string page, string hostname = null, string[] pins = null)
        {
            var secureHttpClientHandler = new SecureHttpClientHandler();
            if (pins != null)
            {
                secureHttpClientHandler.AddCertificatePinner(hostname, pins);
            }
            string result;
            using (var httpClient = new HttpClient(secureHttpClientHandler))
            using (var response = await httpClient.GetAsync(page).ConfigureAwait(false))
            {
                result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return result;
        }
    }
}
