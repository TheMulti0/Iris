FROM python:3.8

MAINTAINER TheMulti0 "multi@codeprecise.com"

COPY facebookscraper /app
WORKDIR /app

RUN pip install --upgrade pip
RUN pip install -r requirements.txt

ENTRYPOINT ["python"]
CMD ["app.py"]