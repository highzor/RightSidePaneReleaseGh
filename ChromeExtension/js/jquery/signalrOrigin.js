/*!
 * ASP.NET SignalR JavaScript Library v2.2.2
 * http://signalr.net/
 *
 * Copyright (c) .NET Foundation. All rights reserved.
 * Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
 *
 */

/// <reference path="..\..\SignalR.Client.JS\Scripts\jquery-1.6.4.js" />
/// <reference path="jquery.signalR.js" />
(function ($, window, undefined) {
    /// <param name="$" type="jQuery" />
    "use strict";

    if (typeof ($.signalR) !== "function") {
        throw new Error("SignalR: SignalR is not loaded. Please ensure jquery.signalR-x.js is referenced before ~/signalr/js.");
    }

    var signalR = $.signalR;

    function makeProxyCallback(hub, callback) {
        return function () {
            // Call the client hub method
            callback.apply(hub, $.makeArray(arguments));
        };
    }

    function registerHubProxies(instance, shouldSubscribe) {
        var key, hub, memberKey, memberValue, subscriptionMethod;

        for (key in instance) {
            if (instance.hasOwnProperty(key)) {
                hub = instance[key];

                if (!(hub.hubName)) {
                    // Not a client hub
                    continue;
                }

                if (shouldSubscribe) {
                    // We want to subscribe to the hub events
                    subscriptionMethod = hub.on;
                } else {
                    // We want to unsubscribe from the hub events
                    subscriptionMethod = hub.off;
                }

                // Loop through all members on the hub and find client hub functions to subscribe/unsubscribe
                for (memberKey in hub.client) {
                    if (hub.client.hasOwnProperty(memberKey)) {
                        memberValue = hub.client[memberKey];

                        if (!$.isFunction(memberValue)) {
                            // Not a client hub function
                            continue;
                        }

                        subscriptionMethod.call(hub, memberKey, makeProxyCallback(hub, memberValue));
                    }
                }
            }
        }
    }

    $.hubConnection.prototype.createHubProxies = function () {
        var proxies = {};
        this.starting(function () {
            // Register the hub proxies as subscribed
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, true);

            this._registerSubscribedHubs();
        }).disconnected(function () {
            // Unsubscribe all hub proxies when we "disconnect".  This is to ensure that we do not re-add functional call backs.
            // (instance, shouldSubscribe)
            registerHubProxies(proxies, false);
        });

        proxies['crmHub'] = this.createHubProxy('crmHub'); 
        proxies['crmHub'].client = { };
        proxies['crmHub'].server = {
            answer: function (callId) {
            /// <summary>Calls the Answer method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"callId\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["Answer"], $.makeArray(arguments)));
             },

            completeCall: function (callId, completeDate, reason) {
            /// <summary>Calls the CompleteCall method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"callId\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"completeDate\" type=\"Object\">Server side type is System.DateTime</param>
            /// <param name=\"reason\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["CompleteCall"], $.makeArray(arguments)));
             },

            deny: function (callId) {
            /// <summary>Calls the Deny method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"callId\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["Deny"], $.makeArray(arguments)));
             },

            hello: function () {
            /// <summary>Calls the Hello method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["Hello"], $.makeArray(arguments)));
             },

            incomingCall: function (callId, date, phoneOfCaller, fullNameOfCaller, dateOfBirthOfCaller, shortNumber) {
            /// <summary>Calls the IncomingCall method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"callId\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"date\" type=\"Object\">Server side type is System.DateTime</param>
            /// <param name=\"phoneOfCaller\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"fullNameOfCaller\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"dateOfBirthOfCaller\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"shortNumber\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["IncomingCall"], $.makeArray(arguments)));
             },

            send: function (name, message) {
            /// <summary>Calls the Send method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"name\" type=\"String\">Server side type is System.String</param>
            /// <param name=\"message\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["Send"], $.makeArray(arguments)));
             },

            signIn: function (inputNumber) {
            /// <summary>Calls the SignIn method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"inputNumber\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["SignIn"], $.makeArray(arguments)));
             },

            signOut: function (inputNumber) {
            /// <summary>Calls the SignOut method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"inputNumber\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["SignOut"], $.makeArray(arguments)));
             },

            summary: function (callId) {
            /// <summary>Calls the Summary method on the server-side CrmHub hub.&#10;Returns a jQuery.Deferred() promise.</summary>
            /// <param name=\"callId\" type=\"String\">Server side type is System.String</param>
                return proxies['crmHub'].invoke.apply(proxies['crmHub'], $.merge(["Summary"], $.makeArray(arguments)));
             }
        };

        return proxies;
    };

    signalR.hub = $.hubConnection("/signalr", { useDefaultPath: false });
    $.extend(signalR, signalR.hub.createHubProxies());

}(window.jQuery, window));