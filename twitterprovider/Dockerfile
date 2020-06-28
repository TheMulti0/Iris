FROM python:3.8

COPY . /app
WORKDIR /app

RUN ls -a

RUN pip install --upgrade pip
RUN pip install -r requirements.txt

ENTRYPOINT ["python"]
CMD ["app.py"]