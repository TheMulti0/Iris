FROM python:3.6

COPY . .

ARG PROJECT_DIR

RUN pip install --upgrade pip
RUN pip install -r $PROJECT_DIR/requirements.txt

ENV PYTHONPATH=.

COPY $PROJECT_DIR/appsettings.json appsettings.json

ENV PROJECT_DIR_ENV=$PROJECT_DIR
ENTRYPOINT python $PROJECT_DIR_ENV