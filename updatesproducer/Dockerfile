FROM python:3.6

WORKDIR /app
COPY . .

ARG PROJECT_DIR

RUN pip install --upgrade pip
RUN pip install -r $PROJECT_DIR/requirements.txt

ENV PYTHONPATH=.

ENV PROJECT_DIR_ENV=$PROJECT_DIR
ENTRYPOINT python $PROJECT_DIR_ENV