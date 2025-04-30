from rest_framework import serializers
from  .models import User 

class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model = User
        fields = '__all__'

class RegistrationSerializer(serializers.Serializer):
    user_name = serializers.CharField(write_only=True)
    password = serializers.CharField(write_only=True)
    role = serializers.HiddenField(default='PATRON')

    def create(self, validated_data):
        user = User.objects.create_user(
            user_name=validated_data['user_name'],
            password=validated_data['password'],
            role=validated_data['role']
        )
        return user

class LoginSerializer(serializers.Serializer):
    user_name = serializers.CharField(write_only=True)
    password = serializers.CharField(write_only=True)

 