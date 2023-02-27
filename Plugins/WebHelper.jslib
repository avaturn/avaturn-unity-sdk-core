mergeInto(LibraryManager.library, {

    ShowAvaturnIframeJS: function () {
        displayIframe();
    },

    HideAvaturnIFrameJS: function () {
        hideIframe();
    },
        
    SetupAvaturnIframeJS: function (link){
        setupIframe(UTF8ToString(link));
    },
});