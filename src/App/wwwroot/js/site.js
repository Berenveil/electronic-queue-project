// Пути к фотографиям внутри wwwroot/images/doctors.
// Слева — ID из базы, а не порядковый номер карточки на странице.
const doctorPhotos = {
    1: "man/1.png",      // Иванов Артём Сергеевич
    2: "woman/1.png",    // Смирнова Анна Дмитриевна
    3: "man/2.png",      // Козлов Максим Александрович
    4: "woman/2.png",    // Петрова Елена Владимировна
    5: "man/3.png",      // Соколов Игорь Николаевич
    6: "woman/3.png",    // Морозова Ольга Игоревна
    7: "man/4.png",      // Лебедев Дмитрий Петрович
    8: "woman/4.png",    // Кузнецова Мария Андреевна
    9: "man/5.png",      // Новиков Алексей Викторович
    10: "woman/5.png",   // Фёдорова Татьяна Сергеевна
    11: "man/6.png",     // Морозов Андрей Игоревич
    12: "woman/6.png",   // Волкова Наталья Алексеевна
    13: "man/7.png",     // Арутюнян Арман Каренович
    14: "woman/7.png",   // Григорян Анна Сааковна
    15: "man/8.png",     // Саркисян Давид Арменович
    16: "woman/8.png",   // Оганесян Лиана Рудольфовна
    17: "woman/9.png",   // Васильева Екатерина Павловна
    18: "woman/10.png",  // Михайлова Юлия Романовна
    19: "man/9.png",     // Алексанян Гарик Вагаршакович
    20: "woman/11.png",  // Петросян Мариам Ашотовна
    21: "woman/12.png",  // Орлова Виктория Андреевна
    22: "man/10.png",    // Павлов Сергей Михайлович
    23: "woman/13.png",  // Акопян Наре Варужановна
    24: "man/11.png",    // Мартиросян Тигран Арташесович
};

const appointmentDialog = document.querySelector("#appointment-dialog");
const appointmentForm = document.querySelector("#appointment-form");
const patientFields = [...appointmentForm.querySelectorAll("input:not([type='hidden'])")];
let appointmentTrigger = null;
let pointerStartedOnBackdrop = false;

function validatePatientField(field) {
    field.setCustomValidity("");
    const value = field.value.trim();
    let message = "";

    if (field.validity.badInput) {
        message = "Введите корректную дату рождения.";
    } else if (field.required && !value) {
        message = "Заполните это поле.";
    } else if (field.name === "fullName" && value.split(/\s+/).length < 2) {
        message = "Укажите фамилию и имя. Отчество — если есть.";
    } else if (field.name === "birthDate" && field.validity.rangeOverflow) {
        message = "Дата рождения не может быть в будущем.";
    } else if (field.name === "phone") {
        const digits = value.replace(/\D/g, "");
        if (!/^\+?[\d\s().-]+$/.test(value) || digits.length < 10 || digits.length > 15) {
            message = "Введите телефон с кодом страны или города, от 10 до 15 цифр.";
        }
    } else if (field.name === "email" && field.validity.typeMismatch) {
        message = "Проверьте адрес почты, например name@example.ru.";
    }

    if (!message && !field.validity.valid) {
        message = field.validationMessage;
    }

    field.setCustomValidity(message);
    field.setAttribute("aria-invalid", String(Boolean(message)));
    const error = document.querySelector(`#${field.id}-error`);
    error.textContent = message;
    error.hidden = !message;
    return !message;
}

function resetAppointmentForm() {
    appointmentForm.reset();
    for (const field of patientFields) {
        field.setCustomValidity("");
        field.removeAttribute("aria-invalid");
        const error = document.querySelector(`#${field.id}-error`);
        error.textContent = "";
        error.hidden = true;
    }
}

function openAppointmentDialog(doctor, trigger) {
    if (appointmentDialog.open) {
        return;
    }

    resetAppointmentForm();
    appointmentTrigger = trigger;
    pointerStartedOnBackdrop = false;
    document.querySelector("#appointment-employee-id").value = doctor.id;
    document.querySelector("#appointment-doctor-name").textContent =
        [doctor.lastName, doctor.firstName, doctor.middleName].filter(Boolean).join(" ");
    document.querySelector("#appointment-doctor-profession").textContent = doctor.profession;

    const today = new Date();
    const month = String(today.getMonth() + 1).padStart(2, "0");
    const day = String(today.getDate()).padStart(2, "0");
    document.querySelector("#patient-birth-date").max = `${today.getFullYear()}-${month}-${day}`;

    appointmentDialog.showModal();
    document.body.classList.add("has-open-dialog");
}

document.querySelector("#appointment-close").addEventListener("click", () => {
    appointmentDialog.close();
});

function isOutsideAppointmentDialog(event) {
    const bounds = appointmentDialog.getBoundingClientRect();
    return event.clientX < bounds.left || event.clientX > bounds.right ||
        event.clientY < bounds.top || event.clientY > bounds.bottom;
}

appointmentDialog.addEventListener("pointerdown", (event) => {
    pointerStartedOnBackdrop = event.target === appointmentDialog && isOutsideAppointmentDialog(event);
});

appointmentDialog.addEventListener("click", (event) => {
    if (pointerStartedOnBackdrop && event.target === appointmentDialog && isOutsideAppointmentDialog(event)) {
        appointmentDialog.close();
    }
    pointerStartedOnBackdrop = false;
});

appointmentDialog.addEventListener("close", () => {
    document.body.classList.remove("has-open-dialog");
    resetAppointmentForm();
    if (appointmentTrigger?.isConnected) {
        appointmentTrigger.focus({ preventScroll: true });
    }
    appointmentTrigger = null;
});

for (const field of patientFields) {
    field.addEventListener("blur", () => {
        if (appointmentDialog.open) {
            validatePatientField(field);
        }
    });
    field.addEventListener("input", () => {
        if (field.getAttribute("aria-invalid") === "true") {
            validatePatientField(field);
        }
    });
}

appointmentForm.addEventListener("submit", (event) => {
    // Выдачу талона подключим после появления API и выбора даты и времени.
    // До этого не отправляем данные, в том числе при нажатии Enter.
    event.preventDefault();
});

async function loadDoctors() {
    const list = document.querySelector("#doctors-list");
    const template = document.querySelector("#doctor-template");
    const status = document.querySelector("#doctors-status");
    const priceFormat = new Intl.NumberFormat("ru-RU");

    status.textContent = "Загружаем врачей…";
    status.hidden = false;
    list.setAttribute("aria-busy", "true");
    list.replaceChildren();

    try {
        const response = await fetch("/api/doctors");

        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        const doctors = await response.json();

        for (const doctor of doctors) {
            const card = template.content.cloneNode(true);

            card.querySelector(".doctor-card").dataset.doctorId = doctor.id;

            const fullName = [
                doctor.lastName,
                doctor.firstName,
                doctor.middleName
            ].filter(Boolean).join(" ");
            card.querySelector(".doctor-name").textContent = fullName;

            const photoFile = doctorPhotos[doctor.id];
            if (photoFile) {
                const photo = card.querySelector(".doctor-photo-image");
                const placeholder = card.querySelector(".doctor-photo-placeholder");

                photo.alt = `Фотография: ${fullName}`;
                photo.addEventListener("load", () => {
                    photo.classList.add("is-loaded");
                    placeholder.hidden = true;
                }, { once: true });
                photo.addEventListener("error", () => {
                    photo.hidden = true;
                    placeholder.hidden = false;
                }, { once: true });
                photo.hidden = false;
                const photoPath = photoFile.split("/").map(encodeURIComponent).join("/");
                photo.src = `/images/doctors/${photoPath}`;
            }

            card.querySelector(".doctor-profession").textContent =
                doctor.profession;

            const primaryService = doctor.services[0];
            card.querySelector(".doctor-price").textContent = primaryService
                ? `${priceFormat.format(primaryService.price)} ₽`
                : "Цена уточняется";
            card.querySelector(".doctor-service").textContent = primaryService
                ? `${primaryService.name} · ${primaryService.durationMin} мин.`
                : "Услуги пока не добавлены";

            const services = card.querySelector(".doctor-services");

            for (const service of doctor.services) {
                const item = document.createElement("li");

                item.dataset.serviceId = service.id;
                item.textContent =
                    `${service.name} — ${priceFormat.format(service.price)} ₽, ` +
                    `${service.durationMin} мин.`;

                services.append(item);
            }

            card.querySelector(".doctor-details").hidden = !doctor.services.length;
            const appointmentButton = card.querySelector(".appointment-button");
            appointmentButton.addEventListener("click", () => {
                openAppointmentDialog(doctor, appointmentButton);
            });
            list.append(card);
        }

        status.textContent = doctors.length ? "" : "Врачи пока не добавлены.";
        status.hidden = doctors.length > 0;
    } catch (error) {
        status.textContent =
            "Не удалось загрузить врачей. Попробуйте обновить страницу.";
        console.error(error);
    } finally {
        list.setAttribute("aria-busy", "false");
    }
}

loadDoctors();
