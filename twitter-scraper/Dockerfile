FROM python:3.8

LABEL maintainer="multi@codeprecise.com"

COPY . /app
WORKDIR /app

RUN pip install --upgrade pip
RUN pip install -r requirements.txt

ENTRYPOINT ["python"]
CMD ["app.py"]