from .repositories import DashboardRepository
from django.core.mail import send_mail


class DashboardService:
    def __init__(self):
        self.repository = DashboardRepository()

    @staticmethod
    def get_dashboard_data():
        repo = DashboardRepository()
        return {
            'total_student_count': repo.get_total_students(),
            'total_book_count': repo.get_total_books(),
            'total_transaction_count': repo.get_total_transactions(),
            'total_borrowed_books': repo.get_borrowed_books_count(),
            'total_returned_books': repo.get_returned_books_count(),
            # 'overdue_borrowers': list(repo.get_overdue_borrowers().values())
            
        }
    
    def get_overdue_borrowers(self):
        return self.repository.get_overdue_borrowers()
    
    def get_overdue_borrowers(self):
        overdue_borrowers = self.repository.get_overdue_borrowers()
        return overdue_borrowers