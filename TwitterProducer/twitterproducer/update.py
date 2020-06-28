from dataclasses import dataclass
from datetime import datetime


@dataclass
class Update:
    content: str
    creation_date: datetime
    url: str
