using System;

namespace qorpent.v2.security.user {
    [Flags]
    public enum UserActivityState {
        Ok = 0,
        Baned = 1,
        Expired = 2,
        MasterBaned = 4,
        MasterExpired = 8,
        InvalidMaster = 16,
        MasterNotChecked = 32,
        InvalidLogonInfo = 64,
        None = -1,
        Error = 128
    }

    public static class UserActivityStateExtensions {
        public static string GetInfo(this UserActivityState state) {
            switch (state) {
                    case UserActivityState.Ok:
                    return "OK";
                    case UserActivityState.Baned:
                    return "Учетная запись отключена";
                    case UserActivityState.Expired:
                    return "Лицензия пользователя завершена";
                    case UserActivityState.InvalidLogonInfo:
                    return "Неверное имя пользователя или пароль";
                    case UserActivityState.InvalidMaster:
                    return "Неверная настройка домена";
                    case UserActivityState.MasterBaned:
                    return "Учетная запись домена отключена";
                    case UserActivityState.MasterExpired:
                    return "Лицензия домена завершена";
                    case UserActivityState.MasterNotChecked:
                    return "Технические проблемы проверки домена";
                    case UserActivityState.None:
                    return "Неопределенный статус";
                    case UserActivityState.Error:
                    return "Ошибка обработки";
                default:
                    throw new Exception("Неизвестный статус "+state);
            }
        }
    }
}