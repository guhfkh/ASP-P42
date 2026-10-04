class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_'); //.replace(/=+$/, '');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));
}

document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == 'auth-form') {
        // зупиняємо автоматичне надсилання форми
        e.preventDefault();
        // вилучаємо дані, що передаються
        const formData = new FormData(form);
        const login = formData.get("auth-login");
        const password = formData.get("auth-password");
        // здійснюємо попередню перевірку (на порожність)
        let errorMessage = "";
        if (login.trim().length === 0) {
            errorMessage += "Логін не може бути порожнім.\n";
        }
        if (password.trim().length === 0) {
            errorMessage += "Пароль не може бути порожнім.\n";
        }
        const err = document.getElementById("auth-modal-error");
        if (errorMessage.length > 0) {
            err.innerText = errorMessage;
            err.style.visibility = "visible";
            return;
        }
        else {
            err.innerText = "";
            err.style.visibility = "hidden";
        }
        // передаємо дані до бекенду з дотриманням стандарту
        // RFC 7617 'Basic' HTTP Authentication Scheme
        // constructs the user-pass by concatenating the user-id, a single
        // colon(":") character, and the password,
        const userPass = login + ':' + password;
        // encodes the user-pass into an octet sequence
        // and obtains the basic - credentials by encoding this octet sequence
        // using Base64 ([RFC4648], Section 4) into a sequence of US - ASCII
        /*
        У JS є вбудовані засоби для Base64, проте, вони не працюють поза
        ASCII символами. Зокрема, непридатні для кирилиці.
        */
        const credentials = Base64.encode(userPass);
        // у запит додається заголовок
        // Authorization: Basic <credentials>
        fetch("/User/BasicAuth", {
            headers: {
                "Authorization": "Basic " + credentials,
            }
        }).then(r => {
            if (r.ok) {
                // return r.json();
                // при роботі з сесіями при позитивній відповіді
                // слід перезавантажити сторінку. Це має активувати
                // роботу Cookie
                window.location.reload();
            }
            else {
                return r.text();
            }
        }).then(console.log);

        // console.log(credentials);
    }
    else if (form.id == 'admin-add-group') {
        e.preventDefault();
        const formData = new FormData(form);
        fetch("/Admin/AddGroup", {
            method: "POST",
            body: formData
        }).then(r => {
            if (r.ok) {
                alert("OK");
                window.location.reload();
            }
            else {
                r.text().then(alert);
            }
        });
    }
    else if (form.id == 'admin-add-product') {
        e.preventDefault();
        const formData = new FormData(form);
        fetch("/Admin/AddProduct", {
            method: "POST",
            body: formData
        }).then(r => {
            //if (r.ok)
            {
                r.text().then(alert);
            }
        });
    }
});
/*
Д.З. Реалізувати відображення помилки даних,
введених у форму автентифікації, у випадку коли
логін містить у своєму складі символ ':', прямо
заборонений стандартом RFC 7617.
* додати поле виведення технічних помилок відповіді сервера
  
*/