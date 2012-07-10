// ShaderDecompiler.h

#pragma once

#include < stdio.h >
#include < stdlib.h >

#include <d3d9.h>
#include <d3dx9.h>
#include <vcclr.h>

#pragma comment( lib, "d3d9.lib" )
#pragma comment( lib, "d3dx9.lib" )

using namespace System;

namespace OpenUO {
	namespace Tools {

	public interface class IEffectCallback
	{
		System::Reflection::Pointer^ GetID3DXEffectInterface(array<Byte> ^ shaderCode);
	};

	public value class ShaderSampler
	{
	public:
		String ^ Name;
		String ^ Semantic;
		String ^ Type;
	};

	public ref class DeviceInterop
	{
	private:
		IDirect3DDevice9 * device;

	public:
		DeviceInterop(System::Reflection::Pointer^ deviceComPtr)
		{
			device = (IDirect3DDevice9 *)System::Reflection::Pointer::Unbox(deviceComPtr);
		}

		bool ZeroShaderConstants()
		{
			unsigned char zero[256*16];
			memset(zero,0,256*16);

			HRESULT hr = 0;
			bool error = false;

			hr = device->SetPixelShaderConstantF(0,(float*)zero,32);	error |= hr != S_OK;
			hr = device->SetPixelShaderConstantB(0,(BOOL*)zero,16);		error |= hr != S_OK;
			hr = device->SetVertexShaderConstantF(0,(float*)zero,256);	error |= hr != S_OK;
			hr = device->SetVertexShaderConstantB(0,(BOOL*)zero,16);	error |= hr != S_OK;

			return !error;
		}

		void GetShaderConstantsPS(array<float> ^constants)
		{
			pin_ptr<float> contents = &constants[0];
			device->GetPixelShaderConstantF(0, contents, constants->Length/4);
		}
		void GetShaderConstantsVS(array<float> ^constants)
		{
			pin_ptr<float> contents = &constants[0];
			device->GetVertexShaderConstantF(0, contents, constants->Length/4);
		}
		void GetShaderConstantsPS(array<BOOL> ^constants)
		{
			pin_ptr<BOOL> contents = &constants[0];
			device->GetPixelShaderConstantB(0, contents, constants->Length);
		}
		void GetShaderConstantsVS(array<BOOL> ^constants)
		{
			pin_ptr<BOOL> contents = &constants[0];
			device->GetVertexShaderConstantB(0, contents, constants->Length);
		}

		enum class Filter
		{
			D3DTEXF_NONE              = 0,
			D3DTEXF_POINT             = 1,
			D3DTEXF_LINEAR            = 2,
			D3DTEXF_ANISOTROPIC       = 3,
		};

		enum class Address
		{
			D3DTADDRESS_WRAP          = 1,
			D3DTADDRESS_MIRROR        = 2,
			D3DTADDRESS_CLAMP         = 3,
		};

		value class State
		{
		public:
			Address U,V,W;
			Filter Min,Mag,Mip;
			DWORD MaxAni;
		};

		void SetTextureFilters(int i, bool vertex)
		{
			if (vertex) i += D3DVERTEXTEXTURESAMPLER0;

			device->SetSamplerState(i, D3DSAMP_ADDRESSU, D3DTADDRESS_WRAP);
			device->SetSamplerState(i, D3DSAMP_ADDRESSV, D3DTADDRESS_WRAP);
			device->SetSamplerState(i, D3DSAMP_ADDRESSW, D3DTADDRESS_WRAP);
				
			device->SetSamplerState(i, D3DSAMP_MAGFILTER, vertex ? D3DTEXF_POINT : D3DTEXF_LINEAR);
			device->SetSamplerState(i, D3DSAMP_MINFILTER, vertex ? D3DTEXF_POINT :D3DTEXF_LINEAR);
			device->SetSamplerState(i, D3DSAMP_MIPFILTER, D3DTEXF_POINT);
				
			device->SetSamplerState(i, D3DSAMP_MAXMIPLEVEL, 0);
			device->SetSamplerState(i, D3DSAMP_MAXANISOTROPY, 1);
		}
		State GetSamplerState(int i, bool vertex)
		{
			if (vertex) i += D3DVERTEXTEXTURESAMPLER0;

			Address u,v,w;
			u = v = w = (Address)D3DTADDRESS_WRAP;

			device->GetSamplerState(i, D3DSAMP_ADDRESSU, (DWORD*)&u);
			device->GetSamplerState(i, D3DSAMP_ADDRESSV, (DWORD*)&v);
			device->GetSamplerState(i, D3DSAMP_ADDRESSW, (DWORD*)&w);
			
			Filter min,mag,mip;
			min = mag = (Filter)D3DTEXF_LINEAR;
			mip = (Filter)D3DTEXF_POINT;
				
			device->GetSamplerState(i, D3DSAMP_MAGFILTER, (DWORD*)&mag);
			device->GetSamplerState(i, D3DSAMP_MINFILTER, (DWORD*)&min);
			device->GetSamplerState(i, D3DSAMP_MIPFILTER, (DWORD*)&mip);
				
			DWORD maxAni = 1;	
			device->GetSamplerState(i, D3DSAMP_MAXANISOTROPY, &maxAni);
			
			State state;
			state.U = u; state.V = v; state.W = w;
			state.Mag = mag; state.Min = min; state.Mip = mip;
			state.MaxAni = maxAni;

			return state;
		}
	};

	//DX shader compiler does not support UTF-8 or the like, so make sure all input files are converted to ASCII... yuck.
	class AsciiIncludeHandler : public ID3DXInclude
	{
		char basePath[1024];
	public:

		AsciiIncludeHandler(String ^ basePath)
		{
			memset(this->basePath, 0, 1024);
			if (basePath->Length > 0)
			{
				array<Byte> ^ path = System::Text::ASCIIEncoding::ASCII->GetBytes(basePath);
				pin_ptr<const Byte> managed = &path[0];
				memcpy(this->basePath, managed, min(1023,basePath->Length));
			}
		}

		HRESULT __stdcall Open(
			D3DXINCLUDE_TYPE IncludeType,
			LPCSTR pFileName,
			LPCVOID pParentData,
			LPCVOID *ppData,
			UINT *pBytes)
		{
			String ^ file = gcnew String(pFileName);

			if (IncludeType == D3DXINC_LOCAL)
			{
				file = System::IO::Path::Combine(gcnew String(basePath), file);
			}

			if (System::IO::File::Exists(file) == false)
				return S_FALSE;

			String ^ contents = System::IO::File::ReadAllText(file);

			array<Byte> ^ ascii = System::Text::ASCIIEncoding::ASCII->GetBytes(contents);

			char * native = (char*)malloc(ascii->Length);
			pin_ptr<const Byte> managed = &ascii[0];

			memcpy(native, managed, ascii->Length);
			
			*ppData = native;
			*pBytes = ascii.Length;

			return S_OK;
		}

		HRESULT __stdcall Close(LPCVOID pData)
		{
			free((void*)pData);
			return S_OK;
		}
	};

	public ref class ShaderDecompiler
	{
		String ^ mErrors;
		String ^ mDisassembledCode;
		array<Byte> ^ mShaderCode;
		array<ShaderSampler> ^ mSamplers;

	public:

		property String ^ Errors
		{
			String ^ get()
			{
				return mErrors;
			}
		}

		property String ^ DisassembledCode
		{
			String ^ get()
			{
				return mDisassembledCode;
			}
		}

		array<Byte> ^ GetShaderCode()
		{
			if (mShaderCode == nullptr)
				return nullptr;
			return (array<Byte>^)mShaderCode->Clone();
		}

		array<ShaderSampler> ^ GetSamplers()
		{
			if (mSamplers == nullptr)
				return nullptr;
			return (array<ShaderSampler>^)mSamplers->Clone();
		}

		ShaderDecompiler(array<Byte> ^ fileContents, String ^ fileName, IEffectCallback ^ callback)
		{
			pin_ptr<const wchar_t> stringw = PtrToStringChars(fileName);
			pin_ptr<const Byte> contents = &fileContents[0];
			ID3DXEffectCompiler * compiler = 0;
			ID3DXBuffer * errors = 0;
			ID3DXBuffer * shaderCode = 0;
			ID3DXBuffer * disassembly = 0;

			D3DXMACRO macros[3];
			macros[0].Name = "WINDOWS";
			macros[0].Definition = "1";
			macros[1].Name = NULL;
			macros[1].Definition = NULL;
			macros[2].Name = NULL;
			macros[2].Definition = NULL;

			AsciiIncludeHandler asciiHandler(System::IO::Path::GetDirectoryName(fileName));

			D3DXCreateEffectCompiler(
				(char*)contents,
				fileContents->Length,
				macros,
				&asciiHandler,
				D3DXSHADER_NO_PRESHADER | D3DXSHADER_OPTIMIZATION_LEVEL0 | D3DXSHADER_USE_LEGACY_D3DX9_31_DLL, 
				&compiler,
				&errors);

			if (errors)
			{
				OnError(errors);
				return;
			}

			compiler->CompileEffect(D3DXSHADER_NO_PRESHADER | D3DXSHADER_OPTIMIZATION_LEVEL1, &shaderCode, &errors);

			if (errors)
			{
				OnError(errors);

				if (shaderCode == NULL)
				{
					compiler->Release();
					return;
				}
			}

			const Byte* data = (Byte*)shaderCode->GetBufferPointer();
			mShaderCode = gcnew array<Byte>(shaderCode->GetBufferSize());
			for (int i=0; i<mShaderCode->Length; i++)
				mShaderCode[i] = data[i];

			if (callback != nullptr)
			{
				ID3DXEffect * effect = (ID3DXEffect*)System::Reflection::Pointer::Unbox(callback->GetID3DXEffectInterface(mShaderCode));

				D3DXDisassembleEffect(effect, 0, &disassembly);

				data = (Byte*)disassembly->GetBufferPointer();
				array<Char> ^ code = gcnew array<Char>(disassembly->GetBufferSize());
				for (int i=0; i<code->Length; i++)
					code[i] = (Char)data[i];
				mDisassembledCode = gcnew String(code);
			}

			//find all the samplers

			System::Collections::Generic::List<ShaderSampler> ^ samplers = gcnew System::Collections::Generic::List<ShaderSampler>();
			int index = 0;
			while (true)
			{
				D3DXHANDLE param = compiler->GetParameter(NULL,index++);
				if (!param) break;

				D3DXPARAMETER_DESC desc;
				compiler->GetParameterDesc(param, &desc);

				ShaderSampler sampler;
				if (desc.Type == D3DXPT_SAMPLER1D) sampler.Type = "Sampler1D";
				if (desc.Type == D3DXPT_SAMPLER2D) sampler.Type = "Sampler2D";
				if (desc.Type == D3DXPT_SAMPLER3D) sampler.Type = "Sampler3D";
				if (desc.Type == D3DXPT_SAMPLERCUBE) sampler.Type = "SamplerCUBE";

				if (sampler.Type != nullptr)
				{
					sampler.Name = gcnew String(desc.Name);
					sampler.Semantic = gcnew String(desc.Semantic);
					samplers->Add(sampler);
				}
			}
			this->mSamplers = samplers->ToArray();

			if (disassembly) disassembly->Release();
			if (shaderCode) shaderCode->Release();
			if (compiler) compiler->Release();
		}

	private:

		void OnError(ID3DXBuffer * errors)
		{
			void * data = errors->GetBufferPointer();
			int size = errors->GetBufferSize();

			mErrors = gcnew String((char*)data,0,size);

			errors->Release();
		}


	public:


	};
	}
}
