using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMusicPlayer
{
	public class MusicParameter : MonoBehaviour
	{
		[SerializeField]
		private string _parameterName = "";
		[SerializeField]
		private float _minValue = 0f;
		public float MinValue
		{
			get { return _minValue; }
			set { _minValue = value; }
		}
		
		[SerializeField]
		private float _maxValue = 100f;
		public float MaxValue
		{
			get { return _maxValue; }
			set { _maxValue = value; }
		}
		
		[SerializeField] 
		private float _currentParameterValue = 50f;
		
		[Tooltip("If any parameter change would result in a gradual change in target")]
		[SerializeField]
		private bool _slew = false;
		[ConditionalHide("_slew", true)] [Tooltip("In parameter units per second")]
		[SerializeField]
		private float _slewRate = 20f;
		public float SlewRate
		{
			get { return _slewRate; }
			set { _slewRate = value; }
		}

		private bool _isSlewing;
		private float _targetParameterValue;
		
		[HideInInspector] public List<ParameterMapping> LinkedMappings = new List<ParameterMapping>();

		private void OnEnable()
		{
			MusicManager.Instance.RegisterParameter(_parameterName, this);
		}

		private void OnDisable()
		{
			MusicManager.Instance.UnRegisterParameter(_parameterName);
		}

		private void OnValidate()
		{
			if (LinkedMappings == null) return; //don't execute in edit mode
			foreach (var mapping in LinkedMappings)
			{
				mapping.ApplyParameterToMusic(_currentParameterValue); //apply the value change to all the mappings
			}
		}

		//to add a mapping
		public void RegisterMapping(ParameterMapping mapping)
		{
			LinkedMappings.Add(mapping);
			if (!mapping.CustomMappingRange) //default mapping range to parameter range
			{
				mapping.MinParameterValue = _minValue;
				mapping.MaxParameterValue = _maxValue;
			}
			else //clamp mapping range to parameter range
			{
				if (mapping.MinParameterValue < _minValue)
					mapping.MinParameterValue = _minValue;
				if (mapping.MaxParameterValue > _maxValue)
					mapping.MaxParameterValue = _maxValue;
			}

			mapping.ApplyParameterToMusic(_currentParameterValue);
		}

		//to remove a mapping
		public void UnRegisterMapping(ParameterMapping mapping)
		{
			LinkedMappings.Remove(mapping);
		}

		public void ChangeTargetValue(float value, bool fromExternal)
		{
			value = Mathf.Clamp(value, _minValue, _maxValue);
			if (fromExternal) _targetParameterValue = value; //if function is called by slew, don't set new target value

			if (!_slew) 
			{
				_currentParameterValue = value;
			}
			else //slewing the parameter change rate
			{
				if (Mathf.Abs(_currentParameterValue - _targetParameterValue) > _slewRate * 0.01f) //if still slewing
				{
					_isSlewing = true;
					//increment the slewing value
					_currentParameterValue += Mathf.Sign(_targetParameterValue - _currentParameterValue) * _slewRate * Time.fixedDeltaTime;                    
				}
				else
				{
					_isSlewing = false;
					_currentParameterValue = _targetParameterValue; //slewing has finished
				}
			}			
			foreach (var mapping in LinkedMappings)
			{
				mapping.ApplyParameterToMusic(_currentParameterValue); //apply the value change to all the mappings
			}
			                     
		}

		private void FixedUpdate()
		{
			if (_isSlewing)
			{
				ChangeTargetValue(_currentParameterValue, false); //continue to slew using the current parameter as input
			}
		}
	}
}