class MongoDbConfig:
    connection_string: str
    db: str

    def __init__(self, json_dict):
        self.__dict__ = json_dict
