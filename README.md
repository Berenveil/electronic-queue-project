# electronic-queue-project

Для запуска нужен .NET SDK 10. Из корня репозитория:

```bash
dotnet run --project src/App
```

Или из папки `src/App`:

```bash
dotnet run
```

Сайт: http://localhost:5207. Проверка сервера: http://localhost:5207/api/health.

База SQLite: `src/App/Data/clinic.db`. При первом запуске применяются
миграции и загружаются данные из `src/App/Data/Seed_all.sql`.
Откройте `clinic.db` в DB Browser for SQLite или расширении SQLite вашего
редактора и выберите таблицу: `Services`, `Employees`, `Clients` и т. д.
В начальном наборе нет записей в `Appointments`.

Локальные базы исключены из Git: каждый разработчик получает свою базу
при запуске. В репозиторий добавляются миграции и SQL с начальными данными.
