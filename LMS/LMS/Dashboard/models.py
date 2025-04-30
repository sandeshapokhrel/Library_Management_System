from django.db import models

from django.db import models
from Student.models import Student
 

class OverdueBorrower(models.Model):
    student = models.ForeignKey(Student, on_delete=models.CASCADE)
    borrowed_id = models.CharField(max_length=100)

    def __str__(self):
        return "__all_"

class Dashboard(models.Model):
    total_student_count = models.IntegerField(default=0)
    total_book_count = models.IntegerField(default=0)
    total_transaction_count = models.IntegerField(default=0)
    total_borrowed_books = models.IntegerField(default=0)
    total_returned_books = models.IntegerField(default=0)
    overdue_borrowers = models.ManyToManyField(OverdueBorrower, related_name="dashboards")

    def __str__(self):
        return "Library Dashboard Statistics"
