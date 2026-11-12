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
