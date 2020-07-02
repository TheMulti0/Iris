class MongoDbConfig:
    connection_string: str

    def __init__(self, json_dict):
        self.__dict__ = json_dict
