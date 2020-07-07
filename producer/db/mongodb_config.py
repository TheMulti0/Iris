class MongoDbConfig:
    connection_string: str
    db: str

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)
