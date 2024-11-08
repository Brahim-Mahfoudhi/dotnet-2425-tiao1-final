window.blazorCulture = {
    get: () => window.localStorage['blazorCulture'],
    set: (value) => window.localStorage['blazorCulture'] = value
};
window.cookieHelper = {
    deleteCookie: function (cookieName, path = "/", domain = "") {
        if (domain) {
            document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=${path}; domain=${domain}`;
        } else {
            document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=${path}`;
        }
    }
};
window.sessionStorage = {
    removeItem: function (key) {
        sessionStorage.removeItem(key);
    },
    setItem: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    getItem: function (key) {
        return sessionStorage.getItem(key);
    },
    clearAll: function () {
        sessionStorage.clear();
    }
}