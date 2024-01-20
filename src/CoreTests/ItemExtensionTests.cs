using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Extensions;
using Nightingale.Core.Workspaces.Models;
using Xunit;

namespace CoreTests
{
    public class ItemExtensionTests
    {
        [Fact]
        public void DistinctHeadersTest()
        {
            var child = new Item();
            child.Headers.Add(new Parameter(false, "test1", "value1", ParamType.Header));

            var parent = new Item();
            parent.Headers.Add(new Parameter(false, "test1", "value2", ParamType.Header));
            parent.Headers.Add(new Parameter(false, "test2", "value2", ParamType.Header));
            parent.Children.Add(child);

            var ancestor = new Item();
            ancestor.Headers.Add(new Parameter(true, "test1", "value3", ParamType.Header));
            ancestor.Headers.Add(new Parameter(true, "test2", "value3", ParamType.Header));
            ancestor.Headers.Add(new Parameter(true, "test3", "value3", ParamType.Header));
            ancestor.Children.Add(parent);

            var headers = child.GetHeaders(activeOnly: false);
            Assert.Equal(3, headers.Count);
            Assert.Contains(headers, x => x.Key == "test1" && x.Value == "value1");
            Assert.Contains(headers, x => x.Key == "test2" && x.Value == "value2");
            Assert.Contains(headers, x => x.Key == "test3" && x.Value == "value3");
        }

        [Fact]
        public void InheritedActiveHeadersTest()
        {
            var child = new Item();

            var parent = new Item();
            parent.Headers.Add(new Parameter(false, "test2", "value2", ParamType.Header));
            parent.Children.Add(child);

            var ancestor = new Item();
            ancestor.Headers.Add(new Parameter(true, "test3", "value3", ParamType.Header));
            ancestor.Children.Add(parent);

            var headers = child.GetInheritedHeaders(activeOnly: true);
            Assert.DoesNotContain(headers, x => x.Key == "test2");
            Assert.Contains(headers, x => x.Key == "test3");
        }

        [Fact]
        public void DistinctQueriesTest()
        {
            var child = new Item();
            child.Url.Queries.Add(new Parameter(false, "test1", "value1", ParamType.Parameter));

            var parent = new Item();
            parent.Url.Queries.Add(new Parameter(false, "test1", "value2", ParamType.Parameter));
            parent.Url.Queries.Add(new Parameter(false, "test2", "value2", ParamType.Parameter));
            parent.Children.Add(child);

            var ancestor = new Item();
            ancestor.Url.Queries.Add(new Parameter(true, "test1", "value3", ParamType.Parameter));
            ancestor.Url.Queries.Add(new Parameter(true, "test2", "value3", ParamType.Parameter));
            ancestor.Url.Queries.Add(new Parameter(true, "test3", "value3", ParamType.Parameter));
            ancestor.Children.Add(parent);

            var queries = child.GetDistinctQueries(activeOnly: false);
            Assert.Equal(3, queries.Count);
            Assert.Contains(queries, x => x.Key == "test1" && x.Value == "value1");
            Assert.Contains(queries, x => x.Key == "test2" && x.Value == "value2");
            Assert.Contains(queries, x => x.Key == "test3" && x.Value == "value3");
        }

        [Fact]
        public void InheritedActiveQueriesTest()
        {
            var child = new Item();

            var parent = new Item();
            parent.Url.Queries.Add(new Parameter(false, "test2", "value2", ParamType.Parameter));
            parent.Children.Add(child);

            var ancestor = new Item();
            ancestor.Url.Queries.Add(new Parameter(true, "test3", "value3", ParamType.Parameter));
            ancestor.Children.Add(parent);

            var queries = child.GetInheritedQueries(activeOnly: true);
            Assert.DoesNotContain(queries, x => x.Key == "test2");
            Assert.Contains(queries, x => x.Key == "test3");
        }

        [Fact]
        public void InheritedInactiveQueriesTest()
        {
            var child = new Item();

            var parent = new Item();
            parent.Url.Queries.Add(new Parameter(false, "test2", "value2", ParamType.Parameter));
            parent.Children.Add(child);

            var ancestor = new Item();
            ancestor.Url.Queries.Add(new Parameter(true, "test3", "value3", ParamType.Parameter));
            ancestor.Children.Add(parent);

            var queries = child.GetInheritedQueries(activeOnly: false);
            Assert.Contains(queries, x => x.Key == "test2");
            Assert.Contains(queries, x => x.Key == "test3");
        }

        /// <summary>
        /// Ensure the TryGetHeader method correctly
        /// retrieves the header value and handles 
        /// disabled/missing keys.
        /// </summary>
        [Fact]
        public void TryGetHeaderTest()
        {
            var item = new Item();
            item.Headers.Add(new Parameter(true, "content-type", "text/xml", ParamType.Header));
            string value = item.TryGetHeader("Content-Type");
            Assert.Equal("text/xml", value);

            item.Headers[0].Enabled = false;
            value = item.TryGetHeader("Content-Type");
            Assert.Null(value);

            item.Headers.Clear();
            value = item.TryGetHeader("Content-Type");
            Assert.Null(value);
        }

        [Fact]
        public void InheritAuthNullParent()
        {
            var child = new Item
            {
                Auth = new Authentication
                {
                    AuthType = AuthType.InheritParent
                }
            };

            var auth = child.GetAuthInheritance();
            Assert.Equal(child.Auth, auth);
            Assert.Equal(AuthType.InheritParent, auth.AuthType);
        }

        [Fact]
        public void InheritAuth0GenerationsUp()
        {
            var p1 = new Item
            {
                Auth = new Authentication
                {
                    AuthType = AuthType.OAuth1
                }
            };

            var child = new Item
            {
                Parent = p1,
                Auth = new Authentication
                {
                    AuthType = AuthType.OAuth2
                }
            };

            var auth = child.GetAuthInheritance();
            Assert.Equal(child.Auth, auth);
            Assert.Equal(AuthType.OAuth2, auth.AuthType);
        }

        [Fact]
        public void InheritAuth1GenerationUp()
        {
            var p3 = new Item
            {
                Auth = new Authentication
                {
                    AuthType = AuthType.None
                }
            };

            var p2 = new Item
            {
                Parent = p3,
                Auth = new Authentication
                {
                    AuthType = AuthType.Basic
                }
            };

            var p1 = new Item
            {
                Parent = p2,
                Auth = new Authentication
                {
                    AuthType = AuthType.OAuth2
                }
            };

            var child = new Item
            {
                Parent = p1,
                Auth = new Authentication
                {
                    AuthType = AuthType.InheritParent
                }
            };

            var auth = child.GetAuthInheritance();
            Assert.Equal(p1.Auth, auth);
            Assert.Equal(AuthType.OAuth2, auth.AuthType);
        }

        [Fact]
        public void InheritAuth2GenerationsUp()
        {
            var p3 = new Item
            {
                Auth = new Authentication
                {
                    AuthType = AuthType.None
                }
            };

            var p2 = new Item
            {
                Parent = p3,
                Auth = new Authentication
                {
                    AuthType = AuthType.Basic
                }
            };

            var p1 = new Item
            {
                Parent = p2,
                Auth = new Authentication
                {
                    AuthType = AuthType.InheritParent
                }
            };

            var child = new Item
            {
                Parent = p1,
                Auth = new Authentication
                {
                    AuthType = AuthType.InheritParent
                }
            };

            var auth = child.GetAuthInheritance();
            Assert.Equal(p2.Auth, auth);
        }
    }
}
