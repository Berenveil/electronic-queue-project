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

            card.querySelector(".doctor-name").textContent = [
                doctor.lastName,
                doctor.firstName,
                doctor.middleName
            ].filter(Boolean).join(" ");

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
