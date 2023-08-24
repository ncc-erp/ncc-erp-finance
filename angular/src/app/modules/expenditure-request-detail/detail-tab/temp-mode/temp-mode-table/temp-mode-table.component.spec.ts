import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TempModeTableComponent } from './temp-mode-table.component';

describe('TempModeTableComponent', () => {
  let component: TempModeTableComponent;
  let fixture: ComponentFixture<TempModeTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TempModeTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TempModeTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
