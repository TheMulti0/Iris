from datetime import datetime

from mongoengine import Document, connect, StringField, DateTimeField, DoesNotExist


class UserLatestUpdateTime(Document):
    user_id: StringField
    latest_update_time: DateTimeField

    def __init__(self, user_id, *args, **values):
        super().__init__(*args, **values)
        self.user_id = user_id
        self.latest_update_time = datetime.min


class UserLatestUpdateTimeRepository:

    def __init__(self):
        connect(
            db='twitterproducer',
            host='localhost',
            port=27017)

    def get_user_latest_update_time(self, user_id):
        document = self.get_matching_document(user_id)
        return document.latest_update_time

    def set_user_latest_update_time(self, user_id, latest_update_time):
        document = self.get_matching_document(user_id)
        document.latest_update_time = latest_update_time
        document.save()

    @staticmethod
    def get_matching_document(user_id):
        documents_count = UserLatestUpdateTime.objects.count()

        new_document = UserLatestUpdateTimeRepository.create_matching_document(
            user_id)

        if documents_count == 0:
            return new_document
        else:
            try:
                return UserLatestUpdateTime.objects(
                    __raw__={'user_id': user_id}).get(0)
            except Exception:
                return new_document

    @staticmethod
    def create_matching_document(user_id):
        return UserLatestUpdateTime(user_id)
