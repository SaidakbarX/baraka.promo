window.copyTextToClipboard = (text) => {
    if (navigator.clipboard && window.isSecureContext) {
        // Безопасный сайт (HTTPS) — используем нормальное API
        return navigator.clipboard.writeText(text);
    } else {
        // Небезопасный сайт (HTTP) — fallback через textarea
        let textArea = document.createElement("textarea");
        textArea.value = text;

        textArea.style.position = "fixed";
        textArea.style.left = "-999999px";
        textArea.style.top = "-999999px";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        return new Promise((resolve, reject) => {
            try {
                if (document.execCommand('copy')) {
                    resolve();
                } else {
                    reject();
                }
            } catch (err) {
                reject(err);
            } finally {
                document.body.removeChild(textArea);
            }
        });
    }
};