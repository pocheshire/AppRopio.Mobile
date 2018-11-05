﻿//
//  Copyright 2018  AppRopio
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
namespace AppRopio.Base.Droid
{
    public class PushConstants
    {
        public const string PUSH_TOKEN_KEY = nameof(PushConstants.PUSH_TOKEN_KEY);

        public const string PUSH_DEEPLINK_KEY = nameof(PushConstants.PUSH_DEEPLINK_KEY);

        public const string PUSH_TITLE_KEY = nameof(PushConstants.PUSH_TITLE_KEY);

        public const string PUSH_BODY_KEY = nameof(PushConstants.PUSH_BODY_KEY);

        public const string METADATA_ICON_KEY = "com.google.firebase.messaging.default_notification_icon";

        public const string METADATA_COLOR_KEY = "com.google.firebase.messaging.default_notification_color";

        public const string METADATA_ACTIVITY_TYPE_KEY = "com.appropio.firebase.messaging.notification_received_activity";
    }
}