import os
from pathlib import Path

# Подхватываем Front/Telegram/.env, если он есть (локальный запуск без Docker).
# Реальные переменные окружения (например, из docker-compose) имеют приоритет.
_env_file = Path(__file__).resolve().parent.parent / ".env"
if _env_file.exists():
    for _line in _env_file.read_text(encoding="utf-8").splitlines():
        _line = _line.strip()
        if _line and not _line.startswith("#") and "=" in _line:
            _key, _, _value = _line.partition("=")
            os.environ.setdefault(_key.strip(), _value.strip().strip('"').strip("'"))

TOKEN = os.environ.get("TELEGRAM_BOT_TOKEN", "")
BOT_USERNAME = os.environ.get("TELEGRAM_BOT_USERNAME", "")
URL = os.environ.get("API_BASE_URL", "http://localhost:5000/api/")
