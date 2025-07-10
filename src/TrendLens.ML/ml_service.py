import pandas as pd
import numpy as np
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import StandardScaler
from flask import Flask, request, jsonify
import joblib
import os

app = Flask(__name__)

class SalesForecastModel:
    def __init__(self):
        self.model = LinearRegression()
        self.scaler = StandardScaler()
        self.is_trained = False
    
    def prepare_features(self, data):
        data['day_of_year'] = data['Date'].dt.dayofyear
        data['month'] = data['Date'].dt.month
        data['day_of_week'] = data['Date'].dt.dayofweek
        return data[['day_of_year', 'month', 'day_of_week']]
    
    def train(self, sales_data):
        df = pd.DataFrame(sales_data)
        df['Date'] = pd.to_datetime(df['Date'])
        
        X = self.prepare_features(df)
        y = df['Amount']
        
        X_scaled = self.scaler.fit_transform(X)
        self.model.fit(X_scaled, y)
        self.is_trained = True
        
        joblib.dump(self.model, 'sales_model.pkl')
        joblib.dump(self.scaler, 'scaler.pkl')
    
    def predict(self, future_dates):
        if not self.is_trained:
            self.load_model()
        
        df = pd.DataFrame({'Date': pd.to_datetime(future_dates)})
        X = self.prepare_features(df)
        X_scaled = self.scaler.transform(X)
        
        predictions = self.model.predict(X_scaled)
        return predictions.tolist()
    
    def load_model(self):
        if os.path.exists('sales_model.pkl'):
            self.model = joblib.load('sales_model.pkl')
            self.scaler = joblib.load('scaler.pkl')
            self.is_trained = True

forecast_model = SalesForecastModel()

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({'status': 'healthy'})

@app.route('/train', methods=['POST'])
def train_model():
    try:
        sales_data = request.json.get('sales_data', [])
        forecast_model.train(sales_data)
        return jsonify({'message': 'Model trained successfully'})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/predict', methods=['POST'])
def predict_sales():
    try:
        future_dates = request.json.get('dates', [])
        predictions = forecast_model.predict(future_dates)
        
        result = [
            {
                'date': date,
                'predicted_amount': float(pred),
                'confidence_interval': float(pred * 0.15)
            }
            for date, pred in zip(future_dates, predictions)
        ]
        
        return jsonify(result)
    except Exception as e:
        return jsonify({'error': str(e)}), 500

@app.route('/customer-value', methods=['POST'])
def predict_customer_value():
    try:
        customer_data = request.json.get('customer_data', {})
        
        avg_order = customer_data.get('average_order_value', 0)
        frequency = customer_data.get('order_frequency', 0)
        
        predicted_clv = avg_order * frequency * 24
        
        return jsonify({
            'predicted_lifetime_value': float(predicted_clv),
            'confidence': 0.8
        })
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001, debug=True)
