from datetime import timedelta, timezone
from Student.models import Student
from Book.models import Book
from Transaction.models import Transaction
from .models import OverdueBorrower 

from django.utils import timezone  # Ensure this import is from Django


class DashboardRepository:
    @staticmethod
    def get_total_students():
        return Student.objects.all().count()

    @staticmethod
    def get_total_books():
        return Book.objects.all().count()

    @staticmethod
    def get_total_transactions():
        return Transaction.all().count()

    @staticmethod
    def get_borrowed_books_count():
        return Transaction.objects.filter(
            transaction_type='borrow'
        ).count()

    @staticmethod
    def get_returned_books_count():
        return Transaction.objects.filter(
            transaction_type='return', 
        ).count()

    @staticmethod
    def get_overdue_borrowers():
        overdue_borrowers = []
        transactions = Transaction.objects.filter(transaction_type="borrow")

        for transaction in transactions:
            overdue_date = transaction.date + timedelta(days=14)  # Calculate overdue date based on transaction date
            
            if timezone.now().date() > overdue_date.date():  # Compare with current date
                overdue_borrower, created = OverdueBorrower.objects.get_or_create(
                    student=transaction.student,
                    borrowed_id=transaction.transaction_id
                )
                overdue_borrowers.append(overdue_borrower)

        return overdue_borrowers