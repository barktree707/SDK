namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to provide handler implementations. The handler
    /// instance will not be released until all objects related to the context have
    /// been destroyed.
    /// </summary>
    public abstract unsafe partial class CefRequestContextHandler
    {
        private cef_cookie_manager_t* get_cookie_manager(cef_request_context_handler_t* self)
        {
            CheckSelf(self);

            var result = GetCookieManager();

            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Called on the browser process IO thread to retrieve the cookie manager. If
        /// this method returns NULL the default cookie manager retrievable via
        /// CefRequestContext::GetDefaultCookieManager() will be used.
        /// </summary>
        protected abstract CefCookieManager GetCookieManager();


        private int on_before_plugin_load(cef_request_context_handler_t* self, cef_string_t* mime_type, cef_string_t* plugin_url, cef_string_t* top_origin_url, cef_web_plugin_info_t* plugin_info, CefPluginPolicy* plugin_policy)
        {
            CheckSelf(self);

            var m_mime_type = cef_string_t.ToString(mime_type);
            var m_plugin_url = cef_string_t.ToString(plugin_url);
            var m_top_origin_url = cef_string_t.ToString(top_origin_url);
            var m_plugin_info = CefWebPluginInfo.FromNative(plugin_info);
            var m_plugin_policy = *plugin_policy;

            var result = OnBeforePluginLoad(m_mime_type, m_plugin_url, m_top_origin_url, m_plugin_info, ref m_plugin_policy);

            *plugin_policy = m_plugin_policy;

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on multiple browser process threads before a plugin instance is
        /// loaded. |mime_type| is the mime type of the plugin that will be loaded.
        /// |plugin_url| is the content URL that the plugin will load and may be empty.
        /// |top_origin_url| is the URL for the top-level frame that contains the
        /// plugin when loading a specific plugin instance or empty when building the
        /// initial list of enabled plugins for 'navigator.plugins' JavaScript state.
        /// |plugin_info| includes additional information about the plugin that will be
        /// loaded. |plugin_policy| is the recommended policy. Modify |plugin_policy|
        /// and return true to change the policy. Return false to use the recommended
        /// policy. The default plugin policy can be set at runtime using the
        /// `--plugin-policy=[allow|detect|block]` command-line flag. Decisions to mark
        /// a plugin as disabled by setting |plugin_policy| to PLUGIN_POLICY_DISABLED
        /// may be cached when |top_origin_url| is empty. To purge the plugin list
        /// cache and potentially trigger new calls to this method call
        /// CefRequestContext::PurgePluginListCache.
        /// </summary>
        protected virtual bool OnBeforePluginLoad(string mimeType, string pluginUrl, string topOriginUrl, CefWebPluginInfo pluginInfo, ref CefPluginPolicy pluginPolicy)
        {
            return false;
        }
    }
}
